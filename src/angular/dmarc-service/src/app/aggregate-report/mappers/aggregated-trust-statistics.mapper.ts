import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class AggregatedTrustStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    let trustedEmailCount = response.values['trusted_email_count'];
    let untrustedEmailCount = response.values['untrusted_email_count'];

    if (trustedEmailCount === 0 && untrustedEmailCount === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = ['Trusted Emails', 'Untrusted Emails'];

    let series: Series[] = [new Series('')];

    series[0].data.push(trustedEmailCount);
    series[0].data.push(untrustedEmailCount);

    return new Dataset(labels, series);
  }
}
