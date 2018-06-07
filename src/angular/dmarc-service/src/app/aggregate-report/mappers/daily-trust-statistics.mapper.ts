import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { DateUtils } from '../../utils/date.utils';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class DailyTrustStatisticsMapper implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    if (Object.keys(response.values).length === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = [];
    let series: Series[] = [
      new Series('Trusted Emails'),
      new Series('Untrusted Emails')
    ];

    for (var date in response.values) {
      labels.push(DateUtils.formatDateString(date));

      series[0].data.push(response.values[date]['trusted_email_count']);
      series[1].data.push(response.values[date]['untrusted_email_count']);
    }

    let dataset = new Dataset(labels, series);

    return dataset;
  }
}
