import {
  Http,
  Response,
  Headers,
  RequestOptions,
  URLSearchParams
} from '@angular/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class DomainSearchService {
  private url;

  private headers = new Headers({ Accept: 'application/json' });

  constructor(private http: Http) {
    this.url = `${window.location.origin}/api/aggregatereport/domains`;
  }

  public getMatchingDomains(searchPattern: string): Observable<any> {
    let urlSearchParams = new URLSearchParams(`searchPattern=${searchPattern}`);

    let options = new RequestOptions({
      headers: this.headers,
      search: urlSearchParams
    });

    return this.http
      .get(this.url, options)
      .map((res: Response) => {
        let result = res.json();
        return result.matches;
      })
      .catch((error: any) => Observable.throw(error || 'Server error'));
  }
}
