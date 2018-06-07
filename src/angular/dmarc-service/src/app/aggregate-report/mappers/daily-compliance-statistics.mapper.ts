import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { DateUtils } from '../../utils/date.utils';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class DailyComplianceStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    if (Object.keys(response.values).length === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = [];
    let series: Series[] = [
      new Series('Fully Compliant'),
      new Series('DKIM Only'),
      new Series('SPF Only')
    ];

    for (var date in response.values) {
      labels.push(DateUtils.formatDateString(date)); // slice to remove the timestamp

      series[0].data.push(response.values[date]['full_compliance_count']);
      series[1].data.push(response.values[date]['dkim_only_count']);
      series[2].data.push(response.values[date]['spf_only_count']);
    }

    return new Dataset(labels, series);
  }
}
