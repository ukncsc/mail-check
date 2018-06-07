import {
  Http,
  Response,
  Headers,
  RequestOptions,
  URLSearchParams
} from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { ResponseMapperInterface } from '../mappers/response-mapper.interface';
import { StatisticsSourceInterface } from './statistics-source.interface';
import { StringUtils } from '../../utils/string.utils';
import { DateUtils } from '../../utils/date.utils';

export class StatisticsService implements StatisticsSourceInterface {
  private url;

  private headers = new Headers({ Accept: 'application/json' });

  constructor(
    url: string,
    protected http: Http,
    protected responseMapper: ResponseMapperInterface
  ) {
    this.url = `${window.location.origin}/api/${url}`;
  }

  getStatistics(
    beginDate: Date,
    endDate: Date,
    domainId: number
  ): Observable<any> {
    let urlSearchParams = new URLSearchParams();
    urlSearchParams.append('beginDateUtc', DateUtils.dateToString(beginDate));
    urlSearchParams.append('endDateUtc', DateUtils.dateToString(endDate));
    if (domainId !== null) {
      urlSearchParams.append('domainId', domainId.toString());
    }

    let options = new RequestOptions({
      headers: this.headers,
      search: urlSearchParams
    });

    if (this.responseMapper == null) {
      return this.http
        .get(this.url, options)
        .map((res: Response) => res.json())
        .catch((error: any) => Observable.throw(error || 'Server error'));
    } else {
      return this.http
        .get(this.url, options)
        .map((res: Response) => this.responseMapper.mapResponse(res.json()))
        .catch((error: any) => Observable.throw(error || 'Server error'));
    }
  }
}
