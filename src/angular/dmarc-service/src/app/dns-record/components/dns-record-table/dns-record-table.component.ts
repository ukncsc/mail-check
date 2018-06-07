import { Component, Input } from '@angular/core';
import { DomainsSecurityInfo } from '../../models/domains-security-info.model';
import { StringUtils } from '../../../utils/string.utils';

@Component({
  selector: 'dns-record-table',
  templateUrl: './dns-record-table.component.html',
  styleUrls: ['./dns-record-table.component.css']
})
export class DnsRecordTableComponent {
  @Input() domainsSecurityInfo: DomainsSecurityInfo;
}
