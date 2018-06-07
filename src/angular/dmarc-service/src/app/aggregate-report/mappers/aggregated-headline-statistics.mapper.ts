import { Injectable } from '@angular/core';
import { HeadlineStatistics } from '../services/aggregated-headline-statistics.model';
import { ResponseMapperInterface } from './response-mapper.interface';

@Injectable()
export class AggregatedHeadlineStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): any {
    let domainCount = response.values['domain_count'];
    let emailCount = response.values['total_email_count'];
    let trustedEmailCount = response.values['trusted_email_count'];
    let untrustedEmailCount = response.values['untrusted_email_count'];
    let fullComplianceCount = response.values['full_compliance_count'];
    let untrustedBlockCount = response.values['untrusted_block_count'];

    if (
      domainCount === 0 &&
      emailCount === 0 &&
      trustedEmailCount === 0 &&
      untrustedEmailCount === 0 &&
      fullComplianceCount === 0 &&
      untrustedBlockCount === 0
    ) {
      return null;
    }

    let trustedPercentage = trustedEmailCount / emailCount * 100;
    let untrustedPercentage = untrustedEmailCount / emailCount * 100;
    let fullyCompliantPercentage = fullComplianceCount / emailCount * 100;
    let untrustedFilteredPercentage = untrustedBlockCount /untrustedEmailCount * 100;
    
    return new HeadlineStatistics(
      domainCount,
      emailCount,
      trustedPercentage,
      untrustedPercentage,
      fullyCompliantPercentage,
      untrustedFilteredPercentage
    );
  }
}
