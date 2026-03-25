import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { WeatherService } from './weather.service';
import { CurrentConditionsComponent } from './current-conditions.component';
import { StatisticsComponent } from './statistics.component';
import { ForecastComponent } from './forecast.component';
import { HeatIndexComponent } from './heat-index.component';
import { SnowComponent } from './snow.component';
import { FireComponent } from './fire.component';

@Component({
  selector: 'app-root',
  imports: [
    FormsModule,
    AsyncPipe,
    CurrentConditionsComponent,
    StatisticsComponent,
    ForecastComponent,
    HeatIndexComponent,
    SnowComponent,
    FireComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  temperature = 80;
  humidity = 65;
  pressure = 1013.1;

  showCurrent = true;
  showStats = true;
  showForecast = true;
  showHeatIndex = true;

  private weatherService = inject(WeatherService);

  // Expose only what the template needs — not the entire service
  subscriberCount$ = this.weatherService.subscriberCount$;
  log$ = this.weatherService.log$;

  clearLog(): void {
    this.weatherService.clearLog();
  }

  setAll(on: boolean): void {
    this.showCurrent = on;
    this.showStats = on;
    this.showForecast = on;
    this.showHeatIndex = on;
  }

  randomizeAndNotify(): void {
    // Nudge each value independently by a small random amount, then notify
    this.temperature = Math.round((this.temperature + (Math.random() * 6 - 3)) * 10) / 10;
    this.humidity = Math.round(Math.min(100, Math.max(0, this.humidity + (Math.random() * 10 - 5))) * 10) / 10;
    this.pressure = Math.round((this.pressure + (Math.random() * 4 - 2)) * 10) / 10;
    this.pushMeasurements();
  }

  pushMeasurements(): void {
    this.weatherService.updateMeasurements(
      this.temperature,
      this.humidity,
      this.pressure
    );
  }
}
