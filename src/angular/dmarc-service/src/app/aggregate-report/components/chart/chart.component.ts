import { Component, Input, ViewChild, Optional } from '@angular/core';
import { Color, BaseChartDirective } from 'ng2-charts';
import { FilterValues } from '../filter/filtervalues.model';
import { StatisticsSourceInterface } from '../../services/statistics-source.interface';
import { Subscription, Observable, Subject } from 'rxjs';
import { Dataset } from '../../mappers/dataset.model';

export enum StatusType {
  NoData,
  Loading,
  Loaded,
  Error
}

@Component({
  selector: 'chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent {
  @Input() public filterValues: Observable<FilterValues>;
  @Input() public dataSource: StatisticsSourceInterface;
  @Input('seriesColours')
  set seriesColours(value: any[]) {
    this.colors = [{ backgroundColor: value }];
  }

  @ViewChild(BaseChartDirective) chart: BaseChartDirective;

  private statisticsService: StatisticsSourceInterface;
  private filterValuesSubscription: Subscription;
  public chartType: string;
  public labels: string[] = [];
  public data: number[] = [];
  public colors: any[];
  public options: any;

  public status: StatusType = StatusType.NoData;

  public statusType = StatusType;

  ngOnInit() {
    this.statisticsService = this.dataSource;

    this.subscribe();
  }

  private subscribe(): void {
    this.filterValuesSubscription = this.filterValues
      .debounceTime(300)
      .distinctUntilChanged()
      .do(val => {
        if (val.clear === true) {
          this.status = StatusType.NoData;
        }
      })
      .filter(val => val.clear !== true)
      .do(() => (this.status = StatusType.Loading))
      .switchMap(val =>
        this.statisticsService.getStatistics(
          val.beginDate,
          val.endDate,
          val.domainId
        )
      )
      .subscribe(
        (data: Dataset) => {
          if (data.series.length === 0) {
            this.status = StatusType.NoData;
          } else {
            this.updateChart(data);
            this.status = StatusType.Loaded;
          }
        },
        (error: any) => {
          this.status = StatusType.Error;
          if (this.filterValuesSubscription) {
            this.filterValuesSubscription.unsubscribe();
            this.filterValuesSubscription = null;
          }
          this.subscribe();
        }
      );
  }

  updateChart(data: Dataset) {
    this.chart.datasets = data.series;
    this.labels = data.labels;
    this.chart.datasets = data.series.map((item, index) => {
      return {
        data: item.data,
        label: item.name,
        backgroundColor: this.colors[index]
      };
    });

    this.labels = data.labels;
    if (this.chart && this.chart.chart && this.chart.chart.config) {
      this.chart.chart.config.data.labels = this.labels;
      this.chart.chart.update();
    }
  }

  ngOnDestroy() {
    this.filterValuesSubscription.unsubscribe();
  }
}
