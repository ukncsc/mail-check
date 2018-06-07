import { Injectable } from '@angular/core';
import { ResponseMapperInterface } from './response-mapper.interface';
import { DateUtils } from '../../utils/date.utils';
import { Dataset, Series } from './dataset.model';

@Injectable()
export class SendersUntrustedMapper implements ResponseMapperInterface {
  mapResponse(response: any): Dataset {
    if (response.length === 0) {
      return new Dataset([], []);
    }

    let labels: string[] = [];
    let series: Series[] = [
      new Series('Untrusted'),
      new Series('Trusted'),
      new Series('DKIM Only'),
      new Series('SPF Only')
    ];

    for (var index = 0; index < response.length; index++) {
      labels.push(response[index].ipAddress);

      series[0].data.push(response[index].untrustedCount);
      series[1].data.push(response[index].trustedCount);
      series[2].data.push(response[index].dkimNoSpfCount);
      series[3].data.push(response[index].spfNoDkimCount);
    }

    return new Dataset(labels, series);
  }
}
