import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { WeatherService } from './weather.service';

@Component({
  selector: 'app-snow',
  templateUrl: './snow.component.html',
  styleUrl: './snow.component.css',
})
export class SnowComponent implements OnInit, OnDestroy {
  snowing = false;
  flakes: { id: number; x: number; speed: number; delay: number; size: number; opacity: number }[] = [];
  private subscription!: Subscription;

  constructor(private weatherService: WeatherService) {
    // Pre-generate snowflakes with random properties
    this.flakes = Array.from({ length: 60 }, (_, i) => ({
      id: i,
      x: Math.random() * 100,
      speed: 3 + Math.random() * 5,
      delay: Math.random() * 5,
      size: 28 + Math.random() * 120,
      opacity: 0.4 + Math.random() * 0.6,
    }));
  }

  ngOnInit(): void {
    this.subscription = this.weatherService.weather$.subscribe((weather) => {
      const hasData = weather.temperature !== 0 || weather.humidity !== 0 || weather.pressure !== 0;
      this.snowing = hasData && weather.temperature <= 32;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
