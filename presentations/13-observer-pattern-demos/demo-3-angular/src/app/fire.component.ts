import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { WeatherService } from './weather.service';

@Component({
  selector: 'app-fire',
  templateUrl: './fire.component.html',
  styleUrl: './fire.component.css',
})
export class FireComponent implements OnInit, OnDestroy {
  burning = false;
  flames: { id: number; x: number; speed: number; delay: number; size: number; opacity: number; waggle: number }[] = [];
  private subscription!: Subscription;

  constructor(private weatherService: WeatherService) {
    this.flames = Array.from({ length: 50 }, (_, i) => ({
      id: i,
      x: Math.random() * 100,
      speed: 2 + Math.random() * 4,
      delay: Math.random() * 4,
      size: 28 + Math.random() * 160,
      opacity: 0.5 + Math.random() * 0.5,
      waggle: 1.5 + Math.random() * 2,
    }));
  }

  ngOnInit(): void {
    this.subscription = this.weatherService.weather$.subscribe((weather) => {
      const hasData = weather.temperature !== 0 || weather.humidity !== 0 || weather.pressure !== 0;
      this.burning = hasData && weather.temperature >= 110;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
