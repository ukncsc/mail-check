import { Injectable } from '@angular/core';
import {
  Http,
  RequestOptions,
  RequestOptionsArgs,
  Response,
  Request,
  Headers,
  XHRBackend
} from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class HttpService extends Http {
  private unauthorised: number = 401;

  constructor(backend: XHRBackend, options: RequestOptions) {
    super(backend, options);
  }

  get(url: string, options?: RequestOptionsArgs): Observable<Response> {
    return super
      .get(url, options)
      .catch((response: Response) => {
        if (response.status == this.unauthorised) {
          window.location.reload(false);
          return Observable.of(response);
        }
        return Observable.throw(response);
      })
      .delayWhen((response: Response) => {
        return Observable.interval(
          response.status == this.unauthorised ? 5000 : 0
        );
      });
  }
}
