import {
  Component,
  OnInit,
  OnDestroy,
  ViewEncapsulation,
  Injectable,
  forwardRef,
} from '@angular/core';
import {
  ActivatedRoute,
  Router,
  Params,
  NavigationExtras,
} from '@angular/router';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';
import { PageService } from '../../services/page.service';
import { SearchService } from '../../services/search.service';
import { Inject } from '@angular/core';
import { StringUtils } from '../../../utils/string.utils';
import { Subscription, Observable, ConnectableObservable, Subject } from 'rxjs';
import { DomainsSecurityInfo } from '../../models/domains-security-info.model';

@Component({
  selector: 'dns-record-dash',
  templateUrl: './dns-record-dash.component.html',
  styleUrls: ['./dns-record-dash.component.css'],
})
export class DnsRecordDashComponent implements OnInit, OnDestroy {
  private queryParamsObs: Observable<Params>;
  private queryParamsSub: Subscription;
  private inputSub: Subscription;

  public domainsSecurityInfo: DomainsSecurityInfo;
  public error: Error;
  public errored: boolean;
  public loading: boolean;

  public page: number = DnsRecordParameters.DEFAULT_PAGE;
  public pageSize: number = DnsRecordParameters.DEFAULT_PAGE_SIZE;
  public search: string = '';

  public constructor(
    private dnsRecordInfoService: DnsRecordInfoService,
    private route: ActivatedRoute,
    private router: Router,
    public pageService: PageService,
    public searchService: SearchService
  ) {}

  public ngOnInit(): void {
    this.queryParamsObs = this.route.queryParams;
    this.inputSub = this.createInputSubscription();
    this.queryParamsSub = this.createQueryParamsSubscription();
  }

  private createInputSubscription(): Subscription {
    let pageObs = this.pageService.page
      .distinctUntilChanged()
      .map(page => new DnsRecordParameters(page, this.pageSize, this.search));

    let searchObs = this.searchService.search
      .debounceTime(300)
      .distinctUntilChanged()
      .map(
        search =>
          new DnsRecordParameters(
            DnsRecordParameters.DEFAULT_PAGE,
            this.pageSize,
            search
          )
      );

    return pageObs.merge(searchObs).subscribe(params => {
      this.search = params.search;
      this.page = params.page;
      this.pageSize = params.pageSize;
      this.navigate(params);
    });
  }

  private createQueryParamsSubscription(): Subscription {
    return this.queryParamsObs
      .do(() => {
        this.loading = true;
        this.domainsSecurityInfo = null;
        this.error = null;
        this.errored = false;
      })
      .debounceTime(300)
      .distinctUntilChanged()
      .map(
        params =>
          new DnsRecordParameters(
            +params['page'],
            +params['pageSize'],
            params['search']
          )
      )
      .do(params => this.ensureParameters(params))
      .filter(params => params.areValid())
      .switchMap(params =>
        this.dnsRecordInfoService.getDomainsSecurityInfo(
          params.page,
          params.pageSize,
          params.search
        )
      )
      .subscribe(_ => {
        if (_.errored) {
          this.errored = true;
          this.error = _.error;
        } else {
          this.errored = false;
          this.domainsSecurityInfo = _.value;
        }
        this.loading = false;
      });
  }

  private ensureParameters(params: DnsRecordParameters) {
    if (!params.areValid()) {
      this.navigate(params);
    }
  }

  private navigate(dnsRecordParameters: DnsRecordParameters): void {
    this.router.navigate(
      ['/anti-spoofing'],
      dnsRecordParameters.getQueryParams()
    );
  }

  public ngOnDestroy(): void {
    this.queryParamsSub.unsubscribe();
    this.inputSub.unsubscribe();
  }
}

export class DnsRecordParameters {
  public static get DEFAULT_PAGE(): number {
    return 1;
  }
  public static get DEFAULT_PAGE_SIZE(): number {
    return 20;
  }

  constructor(
    public page: number,
    public pageSize: number,
    public search: string
  ) {}

  public getQueryParams(): any {
    return StringUtils.isUndefinedNullOrWhitespace(this.search)
      ? {
          queryParams: {
            page: this.getPageOrDefault(),
            pageSize: this.getPageSizeOrDefault(),
          },
        }
      : {
          queryParams: {
            page: this.getPageOrDefault(),
            pageSize: this.getPageSizeOrDefault(),
            search: this.search,
          },
        };
  }

  public areValid() {
    return this.isPageValid() && this.isPageSizeValid();
  }

  private getPageOrDefault(): number {
    return this.isPageValid() ? this.page : DnsRecordParameters.DEFAULT_PAGE;
  }

  private getPageSizeOrDefault(): number {
    return this.isPageSizeValid()
      ? this.pageSize
      : DnsRecordParameters.DEFAULT_PAGE_SIZE;
  }

  private isPageValid() {
    return !isNaN(this.page);
  }

  private isPageSizeValid() {
    return !isNaN(this.pageSize);
  }
}
