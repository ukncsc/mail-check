import { TlsResult } from './tls-result.model';
import { Certificate } from './certificate.model';

export interface MxRecord {
  id: number;
  preference: number;
  hostname: string;
  sslv3: TlsResult;
  tlsv1: TlsResult;
  tlsv11: TlsResult;
  tlsv12: TlsResult;
  certificate: Certificate;
  mxLastChecked: Date;
  tlsLastChecked: Date;
}
