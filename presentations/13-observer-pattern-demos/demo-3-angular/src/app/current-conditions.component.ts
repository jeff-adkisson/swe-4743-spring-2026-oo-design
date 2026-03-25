import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { WeatherService, WeatherData } from './weather.service';

@Component({
  selector: 'app-current-conditions',
  templateUrl: './current-conditions.component.html',
})
export class CurrentConditionsComponent implements OnInit, OnDestroy {
  data: WeatherData = { temperature: 0, humidity: 0, pressure: 0 };
  private subscription!: Subscription;

  constructor(private weatherService: WeatherService) {}

  // subscribe() = register as observer
  ngOnInit(): void {
    this.subscription = this.weatherService.weather$.subscribe(
      (weather) => (this.data = weather)
    );
    this.weatherService.registerSubscriber('CurrentConditionsDisplay');
  }

  // unsubscribe() = prevent lapsed listener leak
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.weatherService.unregisterSubscriber('CurrentConditionsDisplay');
  }
}
