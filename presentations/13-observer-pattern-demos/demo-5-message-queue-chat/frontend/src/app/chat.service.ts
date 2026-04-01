import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, firstValueFrom } from 'rxjs';

export const USERNAME_PATTERN = /^[a-z0-9_-]+$/;

export interface ChatMessage {
  id: string;
  kind: 'chat' | 'system';
  author: string;
  text: string;
  sentAtUtc: string;
}

export interface ChatState {
  status: 'idle' | 'connecting' | 'connected' | 'reconnecting';
  name: string | null;
  queueName: string | null;
  sessionId: string | null;
  error: string | null;
}

interface JoinResponse {
  name: string;
  queueName: string;
  sessionId: string;
}

interface SessionStatusResponse {
  active: boolean;
  name: string;
  queueName: string;
  sessionId: string;
}

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly apiBaseUrl =
    window.location.port === '4200' ? 'http://localhost:8080' : '';

  private readonly stateSubject = new BehaviorSubject<ChatState>({
    status: 'idle',
    name: null,
    queueName: null,
    sessionId: null,
    error: null,
  });

  private readonly messagesSubject = new BehaviorSubject<ChatMessage[]>([]);

  private eventSource: EventSource | null = null;
  private heartbeatHandle: number | null = null;

  readonly state$ = this.stateSubject.asObservable();
  readonly messages$ = this.messagesSubject.asObservable();

  constructor(private readonly http: HttpClient) {
    window.addEventListener('beforeunload', this.leaveOnUnload);
  }

  async initialize(): Promise<void> {
    const sessionId = sessionStorage.getItem('wacky-chat.session-id');

    if (!sessionId) {
      return;
    }

    try {
      const session = await firstValueFrom(
        this.http.get<SessionStatusResponse>(this.apiUrl(`/api/chat/session/${sessionId}`))
      );

      if (!session.active) {
        this.clearClientSession();
        return;
      }

      this.stateSubject.next({
        status: 'connecting',
        name: session.name,
        queueName: session.queueName,
        sessionId: session.sessionId,
        error: null,
      });

      this.startHeartbeat();
      this.connectStream();
    } catch {
      this.clearClientSession();
    }
  }

  async join(rawName: string): Promise<boolean> {
    const name = rawName.trim();

    if (!USERNAME_PATTERN.test(name)) {
      this.patchState({
        error:
          'Usernames may only contain lowercase letters, numbers, dashes, and underscores.',
      });
      return false;
    }

    try {
      this.messagesSubject.next([]);
      this.patchState({
        status: 'connecting',
        error: null,
      });

      const response = await firstValueFrom(
        this.http.post<JoinResponse>(this.apiUrl('/api/chat/join'), { name })
      );

      this.saveClientSession(response);
      this.stateSubject.next({
        status: 'connecting',
        name: response.name,
        queueName: response.queueName,
        sessionId: response.sessionId,
        error: null,
      });

      this.startHeartbeat();
      this.connectStream();
      return true;
    } catch (error) {
      this.clearClientSession();
      this.patchState({
        status: 'idle',
        error: this.extractErrorMessage(error),
      });
      return false;
    }
  }

  async send(rawText: string): Promise<boolean> {
    const sessionId = this.stateSubject.value.sessionId;
    const text = rawText.trim();

    if (!sessionId || text.length === 0) {
      return false;
    }

    try {
      await firstValueFrom(
        this.http.post(this.apiUrl('/api/chat/messages'), { sessionId, text }, { responseType: 'text' })
      );

      this.patchState({ error: null });
      return true;
    } catch (error) {
      this.patchState({ error: this.extractErrorMessage(error) });
      return false;
    }
  }

  async leave(): Promise<void> {
    const sessionId = this.stateSubject.value.sessionId;

    this.stopHeartbeat();
    this.closeStream();

    if (sessionId) {
      try {
        await firstValueFrom(
          this.http.post(this.apiUrl('/api/chat/leave'), { sessionId }, { responseType: 'text' })
        );
      } catch {
        // Ignore leave failures during teardown.
      }
    }

    this.clearClientSession();
    this.stateSubject.next({
      status: 'idle',
      name: null,
      queueName: null,
      sessionId: null,
      error: null,
    });
    this.messagesSubject.next([]);
  }

  private connectStream(): void {
    const sessionId = this.stateSubject.value.sessionId;

    if (!sessionId) {
      return;
    }

    this.closeStream();

    const source = new EventSource(this.apiUrl(`/api/chat/stream/${sessionId}`));

    source.addEventListener('ready', () => {
      this.patchState({
        status: 'connected',
        error: null,
      });
    });

    source.addEventListener('message', (event) => {
      const payload = JSON.parse((event as MessageEvent).data) as ChatMessage;
      this.messagesSubject.next([...this.messagesSubject.value, payload]);
      this.patchState({
        status: 'connected',
        error: null,
      });
    });

    source.onerror = () => {
      if (!this.stateSubject.value.sessionId) {
        return;
      }

      this.patchState({
        status: 'reconnecting',
        error: 'Live stream interrupted. Attempting to reconnect...',
      });
    };

    this.eventSource = source;
  }

  private startHeartbeat(): void {
    if (this.heartbeatHandle !== null) {
      return;
    }

    this.heartbeatHandle = window.setInterval(() => {
      void this.sendHeartbeat();
    }, 10_000);

    void this.sendHeartbeat();
  }

  private stopHeartbeat(): void {
    if (this.heartbeatHandle === null) {
      return;
    }

    window.clearInterval(this.heartbeatHandle);
    this.heartbeatHandle = null;
  }

  private async sendHeartbeat(): Promise<void> {
    const sessionId = this.stateSubject.value.sessionId;

    if (!sessionId) {
      return;
    }

    try {
      await firstValueFrom(
        this.http.post(this.apiUrl('/api/chat/heartbeat'), { sessionId }, { responseType: 'text' })
      );
    } catch {
      // Heartbeats are best effort; EventSource reconnect still handles transient failures.
    }
  }

  private closeStream(): void {
    this.eventSource?.close();
    this.eventSource = null;
  }

  private saveClientSession(session: JoinResponse): void {
    sessionStorage.setItem('wacky-chat.session-id', session.sessionId);
  }

  private clearClientSession(): void {
    sessionStorage.removeItem('wacky-chat.session-id');
  }

  private patchState(patch: Partial<ChatState>): void {
    this.stateSubject.next({
      ...this.stateSubject.value,
      ...patch,
    });
  }

  private apiUrl(path: string): string {
    return `${this.apiBaseUrl}${path}`;
  }

  private extractErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error === 'string' && error.error.trim().length > 0) {
        return error.error;
      }

      if (typeof error.error?.message === 'string') {
        return error.error.message;
      }

      if (error.status === 0) {
        return 'The chat server is unavailable right now.';
      }
    }

    return 'Something went wrong. Please try again.';
  }

  private readonly leaveOnUnload = (): void => {
    const sessionId = this.stateSubject.value.sessionId;

    if (!sessionId || typeof navigator.sendBeacon !== 'function') {
      return;
    }

    const payload = new Blob([JSON.stringify({ sessionId })], {
      type: 'application/json',
    });

    navigator.sendBeacon(this.apiUrl('/api/chat/leave'), payload);
  };
}
