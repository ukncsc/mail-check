import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpModule } from '@angular/http';
import { ChartsModule } from 'ng2-charts';
import { MatProgressSpinnerModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { SharedModule } from '../shared/shared.module';

import { AggregateReportDmarcDashComponent } from './components/aggregate-report-dmarc-dash/aggregate-report-dmarc-dash.component';
import { HeadlineComponent } from './components/headline/headline.component';
import { FilterComponent } from './components/filter/filter.component';
import { AggregatedComplianceStatisticsMapper } from './mappers/aggregated-compliance-statistics.mapper';
import { AggregatedDispositionStatisticsMapper } from './mappers/aggregated-disposition-statistics.mapper';
import { AggregatedHeadlineStatisticsMapper } from './mappers/aggregated-headline-statistics.mapper';
import { AggregatedTrustStatisticsMapper } from './mappers/aggregated-trust-statistics.mapper';
import { DailyComplianceStatisticsMapper } from './mappers/daily-compliance-statistics.mapper';
import { DailyDispositionStatisticsMapper } from './mappers/daily-disposition-statistics.mapper';
import { DailyTrustStatisticsMapper } from './mappers/daily-trust-statistics.mapper';
import { SendersDkimOnlyMapper } from './mappers/senders-dkim-only.mapper';
import { SendersSpfOnlyMapper } from './mappers/senders-spf-only.mapper';
import { SendersTrustedMapper } from './mappers/senders-trusted.mapper';
import { SendersUntrustedMapper } from './mappers/senders-untrusted.mapper';
import { StatisticsServiceFactory } from './factory/statistics-service.factory';
import { DomainSearchService } from './services/domain-search.service';
import { AggregateReportDmarcDashConfig } from './configuration/aggregate-report-dmarc-dash.config';
import { StackedBarChartComponent } from './components/chart/stacked-bar-chart.component';
import { HorizontalStackedBarChartComponent } from './components/chart/horizontal-stacked-bar-chart.component';
import { DoughnutComponent } from './components/chart/doughnut-chart.component';
import { ChartComponent } from './components/chart/chart.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    ChartsModule,
    HttpModule,
    SharedModule,
    MatProgressSpinnerModule,
    BrowserAnimationsModule
  ],
  declarations: [
    AggregateReportDmarcDashComponent,
    HeadlineComponent,
    FilterComponent,
    StackedBarChartComponent,
    HorizontalStackedBarChartComponent,
    DoughnutComponent,
    ChartComponent
  ],
  providers: [
    AggregatedComplianceStatisticsMapper,
    AggregatedDispositionStatisticsMapper,
    AggregatedHeadlineStatisticsMapper,
    AggregatedTrustStatisticsMapper,
    DailyComplianceStatisticsMapper,
    DailyDispositionStatisticsMapper,
    DailyTrustStatisticsMapper,
    SendersDkimOnlyMapper,
    SendersSpfOnlyMapper,
    SendersTrustedMapper,
    SendersUntrustedMapper,
    StatisticsServiceFactory,
    AggregateReportDmarcDashConfig
  ],
  exports: [AggregateReportDmarcDashComponent],
  bootstrap: [AggregateReportDmarcDashComponent]
})
export class AggregateReportModule {}
