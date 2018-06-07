import { Domain } from './domain.model';

export interface DomainSecurityInfo{
    domain : Domain;
    mxRecordCount : number;
    dmarcErrorCount : number;
    dmarcStatus : Status;
    spfErrorCount : number;
    spfStatus : Status;
}

export enum Status{
    Error,
    Warning,
    Info,
    Success,
    Pending,
    None
}
