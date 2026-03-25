import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface WeatherData {
  temperature: number;
  humidity: number;
  pressure: number;
}

@Injectable({ providedIn: 'root' })
export class WeatherService {
  // BehaviorSubject IS the Subject in Observer pattern.
  // It holds the current value and notifies all subscribers when .next() is called.
  private weatherSubject = new BehaviorSubject<WeatherData>({
    temperature: 0,
    humidity: 0,
    pressure: 0,
  });

  // Expose as read-only Observable (observers subscribe to this).
  // asObservable() hides .next() so consumers can observe but not publish.
  weather$: Observable<WeatherData> = this.weatherSubject.asObservable();

  // Tracks how many active subscriptions exist (for the UI counter)
  private subscriberCount = new BehaviorSubject<number>(0);
  subscriberCount$: Observable<number> = this.subscriberCount.asObservable();

  // Event log entries — components and the app shell can subscribe to this
  private logSubject = new BehaviorSubject<string>('');
  log$: Observable<string> = this.logSubject.asObservable();

  private appendLog(entry: string): void {
    const timestamp = new Date().toLocaleTimeString();
    const current = this.logSubject.value;
    this.logSubject.next(`[${timestamp}] ${entry}\n` + current);
  }

  clearLog(): void {
    this.logSubject.next('');
  }

  updateMeasurements(temp: number, humidity: number, pressure: number): void {
    this.appendLog(`Notification({ temperature: ${temp}, humidity: ${humidity}, pressure: ${pressure} })`);
    // .next() triggers notification to ALL subscribers — this is Notify()
    this.weatherSubject.next({ temperature: temp, humidity, pressure });
  }

  registerSubscriber(name: string): void {
    this.subscriberCount.next(this.subscriberCount.value + 1);
    // Defer so the parent's async pipe picks up the log change after
    // the child component's ngOnInit completes within the same CD cycle.
    setTimeout(() => this.appendLog(`${name}.subscribe() — now ${this.subscriberCount.value} observer(s)`));
  }

  unregisterSubscriber(name: string): void {
    this.subscriberCount.next(this.subscriberCount.value - 1);
    setTimeout(() => this.appendLog(`${name}.unsubscribe() — now ${this.subscriberCount.value} observer(s)`));
  }
}
