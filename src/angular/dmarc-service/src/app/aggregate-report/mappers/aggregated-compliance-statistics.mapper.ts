import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class AggregatedComplianceStatisticsMapper
  implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    let fullComplicanceCount = response.values['full_compliance_count'];
    let dkimOnlyCount = response.values['dkim_only_count'];
    let spfOnlyCount = response.values['spf_only_count'];

    if (
      fullComplicanceCount === 0 &&
      dkimOnlyCount === 0 &&
      spfOnlyCount === 0
    ) {
      return new Dataset([], []);
    }

    let labels: string[] = ['Fully Compliant', 'DKIM Only', 'SPF Only'];

    let series: Series[] = [new Series('')];

    series[0].data.push(fullComplicanceCount);
    series[0].data.push(dkimOnlyCount);
    series[0].data.push(spfOnlyCount);

    return new Dataset(labels, series);
  }
}
