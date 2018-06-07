import { Component } from '@angular/core';
import {
  ActivatedRoute,
  Router,
  Params,
  NavigationExtras
} from '@angular/router';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';

@Component({
  selector: 'dns-record-dash-domain',
  templateUrl: './dns-record-dash-domain.component.html',
  styleUrls: ['./dns-record-dash-domain.component.css']
})
export class DnsRecordDashDomainComponent {}
