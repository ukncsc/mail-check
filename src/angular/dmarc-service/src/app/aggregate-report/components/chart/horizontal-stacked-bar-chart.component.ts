import { Component, Input } from '@angular/core';
import { ChartComponent } from './chart.component';

@Component({
  selector: 'horizontal-stacked-bar-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class HorizontalStackedBarChartComponent extends ChartComponent {
  constructor() {
    super();
    this.chartType = 'horizontalBar';

    this.options = {
      scaleShowVerticalLines: false,
      responsive: true,
      scales: {
        xAxes: [
          {
            stacked: true
          }
        ],
        yAxes: [
          {
            stacked: true
          }
        ]
      }
    };
  }

  @Input('seriesColours')
  set seriesColours(value: any[]) {
    this.colors = value;
  }
}
