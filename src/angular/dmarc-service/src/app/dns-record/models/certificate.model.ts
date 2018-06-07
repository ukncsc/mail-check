export interface Certificate {
  thumbPrint: string;
  issuer: string;
  subject: string;
  name: string;
  startDate: Date;
  endDate: Date;
  algorithm: string;
  serialNumber: string;
  version: number;
  valid: boolean;
}
