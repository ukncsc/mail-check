import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class AggregatedDispositionStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    let dispositionNoneCount = response.values['disposition_none_count'];
    let dispositionQuarantineCount =
      response.values['disposition_quarantine_count'];
    let dispositionRejectCount = response.values['disposition_reject_count'];

    if (
      dispositionNoneCount === 0 &&
      dispositionQuarantineCount === 0 &&
      dispositionRejectCount === 0
    ) {
      return new Dataset([], []);
    }

    let labels: string[] = ['None', 'Quarantine', 'Reject'];

    let series: Series[] = [new Series('')];

    series[0].data.push(dispositionNoneCount);
    series[0].data.push(dispositionQuarantineCount);
    series[0].data.push(dispositionRejectCount);

    return new Dataset(labels, series);
  }
}
