import { Component, Input } from '@angular/core';
import { ChartComponent } from './chart.component';

@Component({
  selector: 'doughnut-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class DoughnutComponent extends ChartComponent {
  constructor() {
    super();
    this.chartType = 'doughnut';
  }
}
