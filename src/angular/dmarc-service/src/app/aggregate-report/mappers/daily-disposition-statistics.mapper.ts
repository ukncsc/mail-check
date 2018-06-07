import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { DateUtils } from '../../utils/date.utils';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class DailyDispositionStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    if (Object.keys(response.values).length === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = [];
    let series: Series[] = [
      new Series('None'),
      new Series('Quarantine'),
      new Series('Reject')
    ];

    for (var date in response.values) {
      labels.push(DateUtils.formatDateString(date));
      series[0].data.push(response.values[date]['disposition_none_count']);
      series[1].data.push(
        response.values[date]['disposition_quarantine_count']
      );
      series[2].data.push(response.values[date]['disposition_reject_count']);
    }

    return new Dataset(labels, series);
  }
}
