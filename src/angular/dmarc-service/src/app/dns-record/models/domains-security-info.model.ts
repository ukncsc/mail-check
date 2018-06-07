import { DomainSecurityInfo } from './domain-security-info.model';

export interface DomainsSecurityInfo {
  domainSecurityInfos: DomainSecurityInfo[];
  domainCount: number;
  page: number;
  pageSize: number;
  search: string;
}
