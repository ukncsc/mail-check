import {
  Http,
  Response,
  Headers,
  RequestOptions,
  URLSearchParams
} from '@angular/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class UserService {
  private url;

  private headers = new Headers({ Accept: 'application/json'});

  constructor(private http: Http) {
    this.url = `${window.location.origin}/api/admin/user/current`;
   }

   public isAdmin() : Observable<boolean>{
    let options = new RequestOptions({
      headers: this.headers
    });

    return this.http.get(this.url, options)
      .map(response => response.json().user.roleType === 'Admin')
      .catch(error => Observable.of(false));
   }

   public isAuthorized() : Observable<boolean>{
    let options = new RequestOptions({
      headers: this.headers
    });

    return this.http.get(this.url, options)
      .map(response => response.json().user.roleType !== 'Unauthorised')
      .catch(error => Observable.of(false));
   }
}
