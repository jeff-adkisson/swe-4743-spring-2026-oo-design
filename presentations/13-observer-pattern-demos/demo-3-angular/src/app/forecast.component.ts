import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { WeatherService } from './weather.service';

@Component({
  selector: 'app-forecast',
  templateUrl: './forecast.component.html',
})
export class ForecastComponent implements OnInit, OnDestroy {
  forecast = 'Waiting for data...';
  private lastPressure = 0;
  private currentPressure = 0;
  private subscription!: Subscription;

  constructor(private weatherService: WeatherService) {}

  ngOnInit(): void {
    this.subscription = this.weatherService.weather$.subscribe((weather) => {
      if (weather.pressure === 0) return;
      this.lastPressure = this.currentPressure;
      this.currentPressure = weather.pressure;

      if (weather.temperature >= 110) {
        this.forecast = 'You are cooked!';
      } else if (weather.temperature <= 32) {
        this.forecast = 'Snow!';
      } else if (this.lastPressure === 0) {
        this.forecast = 'Collecting baseline...';
      } else if (this.currentPressure > this.lastPressure) {
        this.forecast = 'Improving weather on the way!';
      } else if (this.currentPressure < this.lastPressure) {
        this.forecast = 'Watch out for cooler, rainy weather.';
      } else {
        this.forecast = 'More of the same.';
      }
    });
    this.weatherService.registerSubscriber('ForecastDisplay');
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.weatherService.unregisterSubscriber('ForecastDisplay');
  }
}
