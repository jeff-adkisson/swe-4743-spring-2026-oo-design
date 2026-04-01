import { AsyncPipe, DatePipe } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ChatService, USERNAME_PATTERN } from './chat.service';

@Component({
  selector: 'app-root',
  imports: [AsyncPipe, DatePipe, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit, AfterViewInit {
  joinName = '';
  draftMessage = '';

  protected readonly usernamePattern = USERNAME_PATTERN;
  protected readonly chatService = inject(ChatService);

  readonly state$ = this.chatService.state$;
  readonly messages$ = this.chatService.messages$;

  @ViewChild('joinNameInput')
  private joinNameInput?: ElementRef<HTMLInputElement>;

  @ViewChild('messageInput')
  private messageInput?: ElementRef<HTMLInputElement>;

  private readonly elementRef = inject(ElementRef<HTMLElement>);
  private viewInitialized = false;
  private pendingFocusTarget: 'join' | 'message' = 'join';

  constructor() {
    let previousSessionId: string | null | undefined = undefined;

    this.state$
      .pipe(takeUntilDestroyed())
      .subscribe((state) => {
        if (previousSessionId === state.sessionId) {
          return;
        }

        previousSessionId = state.sessionId;
        this.scheduleInputFocus(state.sessionId ? 'message' : 'join');
      });

    this.messages$
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        requestAnimationFrame(() => this.scrollToBottom());
      });
  }

  async ngOnInit(): Promise<void> {
    await this.chatService.initialize();
  }

  ngAfterViewInit(): void {
    this.viewInitialized = true;
    this.scheduleInputFocus(this.pendingFocusTarget);
  }

  async joinChat(): Promise<void> {
    const joined = await this.chatService.join(this.joinName);

    if (joined) {
      this.joinName = '';
      this.draftMessage = '';
    }
  }

  async sendMessage(): Promise<void> {
    const sent = await this.chatService.send(this.draftMessage);

    if (sent) {
      this.draftMessage = '';
    }
  }

  async leaveChat(): Promise<void> {
    await this.chatService.leave();
    this.draftMessage = '';
  }

  normalizeJoinName(rawValue: string): void {
    this.joinName = rawValue.toLowerCase().replace(/[^a-z0-9_-]/g, '');
  }

  private scrollToBottom(): void {
    const messageLog = this.elementRef.nativeElement.querySelector('[data-message-log]') as HTMLElement | null;

    if (messageLog) {
      messageLog.scrollTop = messageLog.scrollHeight;
    }
  }

  private scheduleInputFocus(target: 'join' | 'message'): void {
    this.pendingFocusTarget = target;

    if (!this.viewInitialized) {
      return;
    }

    requestAnimationFrame(() => {
      const input =
        target === 'message'
          ? this.messageInput?.nativeElement
          : this.joinNameInput?.nativeElement;

      input?.focus();
    });
  }
}
