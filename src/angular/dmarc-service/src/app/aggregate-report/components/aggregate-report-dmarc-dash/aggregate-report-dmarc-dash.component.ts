import {
  Component,
  Output,
  OnInit,
  EventEmitter,
  OnDestroy
} from '@angular/core';
import { Router, Params } from '@angular/router';
import { FilterValues } from '../filter/filtervalues.model';
import { Subscription, Observable, Subject } from 'rxjs';
import { DateUtils } from '../../../utils/date.utils';
import { StringUtils } from '../../../utils/string.utils';
import { UserService } from '../../../user.service'
import { AggregateReportDmarcDashConfig } from '../../configuration/aggregate-report-dmarc-dash.config';
import * as moment from 'moment';

@Component({
  selector: 'aggregate-report-dmarc-dash',
  templateUrl: './aggregate-report-dmarc-dash.component.html',
  styleUrls: ['./aggregate-report-dmarc-dash.component.css']
})
export class AggregateReportDmarcDashComponent implements OnInit, OnDestroy {
  @Output() filterValues: EventEmitter<FilterValues> = new EventEmitter();

  private queryParamsSubscription: Subscription;
  private filterValuesSubscription: Subscription;
  private authorisedSubscription: Subscription
  private calledEnsureParams: boolean;

  public filterValuesObs = new Subject<FilterValues>();
  public isAuthorised : boolean = false;
  public isAuthorisedLoaded: boolean = false;

  constructor(
    private router: Router,
    private userService: UserService,
    public config: AggregateReportDmarcDashConfig
  ) {
    this.calledEnsureParams = false;
  }

  ngOnInit() {
    this.authorisedSubscription = this.userService.isAuthorized().subscribe((isAuthorised) =>{
      this.isAuthorised = isAuthorised;
      this.isAuthorisedLoaded = true;
    });

    this.queryParamsSubscription = this.router.routerState.root.queryParams.subscribe(
      (params: Params) => this.ensureParams(params)
    );

    this.filterValuesSubscription = this.filterValuesObs.subscribe(
      (filterValues: FilterValues) => this.filterValues.emit(filterValues)
    );
  }

  ngOnDestroy() {
    this.authorisedSubscription.unsubscribe();
    this.queryParamsSubscription.unsubscribe();
    this.filterValuesSubscription.unsubscribe();
  }

  private ensureParams(params: Params) {
    // latch this call and unsubscribe on in ngOnDestroy
    // because can unsubscribe in a subscription
    if (this.calledEnsureParams === false) {
      let beginDateParam: string = params['beginDate'];
      let endDateParam: string = params['endDate'];
      let domainParam: string = params['domain'];
      if (
        StringUtils.isUndefinedNullOrWhitespace(beginDateParam) &&
        StringUtils.isUndefinedNullOrWhitespace(endDateParam) &&
        StringUtils.isUndefinedNullOrWhitespace(domainParam)
      ) {
        const beginDate = moment()
          .utc()
          .subtract(7, 'days');
        const endDate = moment.utc();

        this.router.navigate(['/email-abuse'], {
          queryParams: {
            beginDate: beginDate.format('YYYY-MM-DD'),
            endDate: endDate.format('YYYY-MM-DD')
          }
        });
      }
      this.calledEnsureParams = true;
    }
  }
}
