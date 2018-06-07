import { Observable } from 'rxjs/Rx';

export interface StatisticsSourceInterface {
  getStatistics(
    beginDate: Date,
    endDate: Date,
    domainId: number
  ): Observable<any>;
}
