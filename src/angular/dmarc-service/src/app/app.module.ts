import { BrowserModule } from '@angular/platform-browser';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { NgModule } from '@angular/core';
import { Http } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatProgressSpinnerModule } from '@angular/material';
import { ChartsModule } from 'ng2-charts';

import { AggregateReportModule } from './aggregate-report/aggregate-report.module';
import { DnsRecordModule } from './dns-record/dns-record.module';

import { AppComponent } from './app.component';
import { FooterComponent } from './footer/footer.component';
import { TermsAndConditionsComponent } from './terms-and-conditions/terms-and-conditions.component';

import { routing } from './app.routing';
import { HttpService } from './http.service';
import { BannerComponent } from './banner/banner.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { UserService } from './user.service';

@NgModule({
  declarations: [
    AppComponent,
    FooterComponent,
    BannerComponent,
    NavBarComponent,
    TermsAndConditionsComponent,
  ],
  imports: [
    BrowserModule,
    NgbModule.forRoot(),
    routing,
    AggregateReportModule,
    DnsRecordModule,
    ChartsModule,
    MatProgressSpinnerModule,
  ],
  providers: [
    { provide: LocationStrategy, useClass: HashLocationStrategy },
    { provide: Http, useClass: HttpService },
    { provide: UserService, useClass: UserService },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
