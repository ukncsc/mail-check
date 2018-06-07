import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class SearchService {
  public search: Subject<string> = new Subject<string>();
}
