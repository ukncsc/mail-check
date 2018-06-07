import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { StatisticsServiceFactory } from '../../factory/statistics-service.factory';
import { HeadlineStatistics } from '../../services/aggregated-headline-statistics.model';
import { FilterValues } from '../filter/filtervalues.model';
import { StatisticsSourceInterface } from '../../services/statistics-source.interface';
import { Subscription, Observable, Subject } from 'rxjs';
import { LargeNumberPipe } from '../../../shared/pipes/large-number.pipe';

export enum StatusType {
  NoData,
  Loading,
  Loaded,
  Error
}

@Component({
  selector: 'headline',
  templateUrl: './headline.component.html',
  styleUrls: ['./headline.component.css'],
  providers: [StatisticsServiceFactory]
})
export class HeadlineComponent implements OnInit, OnDestroy {
  @Input() public filterValues: Observable<FilterValues>;
  @Input() public dataSource: StatisticsSourceInterface;

  private statisticsService: StatisticsSourceInterface;
  private filterValuesSubscription: Subscription;

  public headlineStatistics: HeadlineStatistics = new HeadlineStatistics(
    0,
    0,
    0,
    0,
    0,
    0
  );

  public status: StatusType = StatusType.NoData;

  statusType = StatusType;

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
        (headlineStatistics: HeadlineStatistics) => {
          if (headlineStatistics === null) {
            this.status = StatusType.NoData;
          } else {
            this.status = StatusType.Loaded;
            this.headlineStatistics = headlineStatistics;
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

  ngOnDestroy() {
    this.filterValuesSubscription.unsubscribe();
  }
}
