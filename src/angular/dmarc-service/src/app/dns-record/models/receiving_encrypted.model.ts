import { Domain } from './domain.model';
import { MxRecord } from './mx-record.model';

export interface ReceivingEncrypted {
  domain: Domain;
  mxRecords: MxRecord[];
}
