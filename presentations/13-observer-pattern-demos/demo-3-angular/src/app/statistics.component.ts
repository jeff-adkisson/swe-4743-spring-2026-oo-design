import { Component, OnInit, OnDestroy } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Subscription } from 'rxjs';
import { WeatherService } from './weather.service';

@Component({
  selector: 'app-statistics',
  imports: [DecimalPipe],
  templateUrl: './statistics.component.html',
})
export class StatisticsComponent implements OnInit, OnDestroy {
  avg = 0;
  min = Infinity;
  max = -Infinity;
  readings: number[] = [];
  private subscription!: Subscription;

  constructor(private weatherService: WeatherService) {}

  ngOnInit(): void {
    this.subscription = this.weatherService.weather$.subscribe((weather) => {
      if (weather.temperature === 0 && weather.humidity === 0) return;
      this.readings.push(weather.temperature);
      this.avg =
        this.readings.reduce((a, b) => a + b, 0) / this.readings.length;
      this.min = Math.min(...this.readings);
      this.max = Math.max(...this.readings);
    });
    this.weatherService.registerSubscriber('StatisticsDisplay');
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.weatherService.unregisterSubscriber('StatisticsDisplay');
  }
}
