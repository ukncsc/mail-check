import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class PageService {
  public page: Subject<number> = new Subject<number>();
}
