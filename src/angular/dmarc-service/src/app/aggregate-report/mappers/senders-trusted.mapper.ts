import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { DateUtils } from '../../utils/date.utils';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class SendersTrustedMapper implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    if (response.length === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = [];
    let series: Series[] = [
      new Series('Trusted'),
      new Series('DKIM Only'),
      new Series('SPF Only'),
      new Series('Untrusted')
    ];

    for (var index = 0; index < response.length; index++) {
      labels.push(response[index].ipAddress);

      series[0].data.push(response[index].trustedCount);
      series[1].data.push(response[index].dkimNoSpfCount);
      series[2].data.push(response[index].spfNoDkimCount);
      series[3].data.push(response[index].untrustedCount);
    }

    return new Dataset(labels, series);
  }
}
