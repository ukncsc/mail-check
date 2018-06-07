import { Domain } from './domain.model';
import { DmarcRecord } from './dmarc-record.model';
import { SpfRecord } from './spf-record.model';

export interface AntiSpoofing {
  domain: Domain;
  dmarcRecords: DmarcRecord[];
  dmarcRecordConfigValid: boolean;
  spfRecords: SpfRecord[];
  spfRecordConfigValid: boolean;
}
