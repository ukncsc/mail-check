import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterModule} from '@angular/router';


import { DnsRecordDashComponent } from './components/dns-record-dash/dns-record-dash.component';

import { DnsRecordInfoService} from './services/dns-record-info.service';
import { PaginationDisplayComponent } from './components/pagination-display/pagination-display.component';
import { DnsRecordTableComponent } from './components/dns-record-table/dns-record-table.component';
import { DnsRecordSearchComponent } from './components/dns-record-search/dns-record-search.component';

import { PageService, } from './services/page.service';
import { SearchService, } from './services/search.service';
import { DnsRecordDashDomainComponent } from './components/dns-record-dash-domain/dns-record-dash-domain.component';
import { DomainSecurityHeaderComponent } from './components/domain-security-header/domain-security-header.component';

import { MatProgressSpinnerModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SpfComponent } from './components/spf/spf.component';
import { DmarcComponent } from './components/dmarc/dmarc.component';
import { LongTagSplitterPipe } from './long-tag-splitter.pipe';
import { TlsComponent } from './components/tls/tls.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    MatProgressSpinnerModule,
    BrowserAnimationsModule
  ],
  declarations: [
    DnsRecordDashComponent,
    PaginationDisplayComponent,
    DnsRecordTableComponent,
    DnsRecordSearchComponent,
    DnsRecordDashDomainComponent,
    DomainSecurityHeaderComponent,
    SpfComponent,
    DmarcComponent,
    LongTagSplitterPipe,
    TlsComponent,
  ],
  providers: [
    DnsRecordInfoService,
    PageService,
    SearchService,
  ],
  exports:[
    DnsRecordDashComponent
  ],
  bootstrap: [DnsRecordDashComponent]
})
export class DnsRecordModule { }
