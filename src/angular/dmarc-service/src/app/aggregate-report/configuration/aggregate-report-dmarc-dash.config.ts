import { StatisticsSourceInterface } from '../services/statistics-source.interface';
import { StatisticsServiceFactory } from '../factory/statistics-service.factory';
import { Injectable } from '@angular/core';

export class AggregateReportDmarcDashConfigItem {
  constructor(
    public title: string,
    public dataSource: () => StatisticsSourceInterface
  ) {}
}

export interface LayoutConfigItem {
  title: string;
  width: number;
  chartType: string;
  seriesColours: any[];
  dataSource: StatisticsSourceInterface;
}

@Injectable()
export class AggregateReportDmarcDashConfig {
  constructor(private statisticsServiceFactory: StatisticsServiceFactory) {}

  public Config: LayoutConfigItem[] = [
    {
      title: 'Trust',
      width: 4,
      chartType: 'doughnut',
      seriesColours: ['#3F9693', '#77286E'],
      dataSource: this.statisticsServiceFactory.createAggregatedTrustStatisticSerivce()
    },
    {
      title: 'Compliance',
      width: 4,
      chartType: 'doughnut',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createAggregatedComplianceStatisticSerivce()
    },
    {
      title: 'Disposition',
      width: 4,
      chartType: 'doughnut',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createAggregatedDispositionStatisticSerivce()
    },
    {
      title: 'Top Trusted Senders',
      width: 6,
      chartType: 'horizontalBar',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9', '#C28A13'],
      dataSource: this.statisticsServiceFactory.createTopTrustedSendersStatisticsService()
    },
    {
      title: 'Top Dkim Only Sender',
      width: 6,
      chartType: 'horizontalBar',
      seriesColours: ['#77286E', '#3F9693', '#2B70B9', '#C28A13'],
      dataSource: this.statisticsServiceFactory.createTopDkimNoSpfSendersStatisticsService()
    },
    {
      title: 'Top Spf Only Senders',
      width: 6,
      chartType: 'horizontalBar',
      seriesColours: ['#2B70B9', '#3F9693', '#77286E', '#C28A13'],
      dataSource: this.statisticsServiceFactory.createTopSpfNoDkimSendersStatisticsService()
    },
    {
      title: 'Top Untrusted Senders',
      width: 6,
      chartType: 'horizontalBar',
      seriesColours: ['#C28A13', '#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createTopUntrustedSendersStatisticsService()
    },
    {
      title: 'Trust',
      width: 12,
      chartType: 'bar',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createDailyTrustStatisticSerivce()
    },
    {
      title: 'Compliance',
      width: 12,
      chartType: 'bar',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createDailyComplianceStatisticSerivce()
    },
    {
      title: 'Disposition',
      width: 12,
      chartType: 'bar',
      seriesColours: ['#3F9693', '#77286E', '#2B70B9'],
      dataSource: this.statisticsServiceFactory.createDailyDispositionStatisticSerivce()
    }
  ];

  public Headline: AggregateReportDmarcDashConfigItem = new AggregateReportDmarcDashConfigItem(
    '',
    () =>
      this.statisticsServiceFactory.createAggregatedHeadlineStatisticSerivce()
  );
}
