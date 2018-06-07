import { Routes, RouterModule } from '@angular/router';
import { AggregateReportDmarcDashComponent } from './aggregate-report/components/aggregate-report-dmarc-dash/aggregate-report-dmarc-dash.component';
import { DnsRecordDashComponent } from './dns-record/components/dns-record-dash/dns-record-dash.component';
import { DnsRecordDashDomainComponent } from './dns-record/components/dns-record-dash-domain/dns-record-dash-domain.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';

export const routes: Routes = [
  { path: '', redirectTo: 'email-abuse', pathMatch: 'full' },
  { path: 'email-abuse', component: AggregateReportDmarcDashComponent },
  { path: 'domain-security/domain/:id', component: DnsRecordDashDomainComponent },
  { path: 'terms', component: TermsAndConditionsComponent },
];

export const routing = RouterModule.forRoot(routes);
