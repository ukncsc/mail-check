import { Http } from '@angular/http';
import { Injectable } from '@angular/core';
import { StatisticsService } from '../services/statistics-service';
import { StatisticsSourceInterface } from '../services/statistics-source.interface';
import { AggregatedComplianceStatisticsMapper } from '../mappers/aggregated-compliance-statistics.mapper';
import { AggregatedDispositionStatisticsMapper } from '../mappers/aggregated-disposition-statistics.mapper';
import { AggregatedHeadlineStatisticsMapper } from '../mappers/aggregated-headline-statistics.mapper';
import { AggregatedTrustStatisticsMapper } from '../mappers/aggregated-trust-statistics.mapper';
import { DailyComplianceStatisticsMapper } from '../mappers/daily-compliance-statistics.mapper';
import { DailyDispositionStatisticsMapper } from '../mappers/daily-disposition-statistics.mapper';
import { DailyTrustStatisticsMapper } from '../mappers/daily-trust-statistics.mapper';
import { SendersDkimOnlyMapper } from '../mappers/senders-dkim-only.mapper';
import { SendersSpfOnlyMapper } from '../mappers/senders-spf-only.mapper';
import { SendersTrustedMapper } from '../mappers/senders-trusted.mapper';
import { SendersUntrustedMapper } from '../mappers/senders-untrusted.mapper';

@Injectable()
export class StatisticsServiceFactory {
  constructor(
    private http: Http,
    private aggregatedComplianceStatisticsMapper: AggregatedComplianceStatisticsMapper,
    private aggregatedDispositionStatisticsMapper: AggregatedDispositionStatisticsMapper,
    private aggregatedHeadlineStatisticsMapper: AggregatedHeadlineStatisticsMapper,
    private aggregatedTrustStatisticsMapper: AggregatedTrustStatisticsMapper,
    private dailyComplianceStatisticsMapper: DailyComplianceStatisticsMapper,
    private dailyDispositionStatisticsMapper: DailyDispositionStatisticsMapper,
    private dailyTrustStatisticsMapper: DailyTrustStatisticsMapper,
    private sendersDkimOnlyMapper: SendersDkimOnlyMapper,
    private sendersSpfOnlyMapper: SendersSpfOnlyMapper,
    private sendersTrustedMapper: SendersTrustedMapper,
    private sendersUntrustedMapper: SendersUntrustedMapper
  ) {}

  createAggregatedComplianceStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/aggregated/compliance',
      this.http,
      this.aggregatedComplianceStatisticsMapper
    );
  }

  createAggregatedDispositionStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/aggregated/disposition',
      this.http,
      this.aggregatedDispositionStatisticsMapper
    );
  }

  createAggregatedHeadlineStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/aggregated/headline',
      this.http,
      this.aggregatedHeadlineStatisticsMapper
    );
  }

  createAggregatedTrustStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/aggregated/trust',
      this.http,
      this.aggregatedTrustStatisticsMapper
    );
  }

  createDailyComplianceStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/daily/compliance',
      this.http,
      this.dailyComplianceStatisticsMapper
    );
  }

  createDailyDispositionStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/daily/disposition',
      this.http,
      this.dailyDispositionStatisticsMapper
    );
  }

  createDailyTrustStatisticSerivce(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/daily/trust',
      this.http,
      this.dailyTrustStatisticsMapper
    );
  }

  createTopTrustedSendersStatisticsService(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/senders/trusted',
      this.http,
      this.sendersTrustedMapper
    );
  }

  createTopDkimNoSpfSendersStatisticsService(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/senders/dkimnospf',
      this.http,
      this.sendersDkimOnlyMapper
    );
  }

  createTopSpfNoDkimSendersStatisticsService(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/senders/spfnodkim',
      this.http,
      this.sendersSpfOnlyMapper
    );
  }

  createTopUntrustedSendersStatisticsService(): StatisticsSourceInterface {
    return new StatisticsService(
      'aggregatereport/senders/untrusted',
      this.http,
      this.sendersUntrustedMapper
    );
  }
}
