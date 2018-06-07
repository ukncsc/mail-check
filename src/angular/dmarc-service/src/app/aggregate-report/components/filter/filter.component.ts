import {
  Component,
  Output,
  EventEmitter,
  OnInit,
  OnDestroy,
} from '@angular/core';
import {
  FormControl,
  Validators,
  FormGroup,
  FormBuilder,
} from '@angular/forms';
import { Router, ActivatedRoute, Params } from '@angular/router';
import {
  NgbDateStruct,
  NgbTypeaheadSelectItemEvent,
} from '@ng-bootstrap/ng-bootstrap';
import { FilterValues } from './filtervalues.model';
import { Subscription, Observable, Subject } from 'rxjs';
import { StringUtils } from '../../../utils/string.utils';
import { DateUtils } from '../../../utils/date.utils';
import { DomainSearchService } from '../../services/domain-search.service';
import { trim } from 'lodash';

@Component({
  selector: 'filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css'],
  providers: [DomainSearchService],
})
export class FilterComponent implements OnInit, OnDestroy {
  @Output() filterValues: EventEmitter<FilterValues> = new EventEmitter();

  public selectedItemObs = new Subject<NgbTypeaheadSelectItemEvent>();

  private formSubscription: Subscription;
  private queryParamsSubscription: Subscription;
  private selectedItemSubscription: Subscription;
  private calledHandleChangesToQueryParameters: boolean;
  private selectedItem: any;

  public filterForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private domainSearchService: DomainSearchService
  ) {
    this.calledHandleChangesToQueryParameters = false;
    this.selectedItem = null;
  }

  ngOnInit() {
    //create form
    this.filterForm = this.createForm();

    //subscribe to changes
    this.formSubscription = this.filterForm.valueChanges
      .filter(val => this.filterForm.valid)
      .map(val => this.createFilterValues(val))
      .debounceTime(500)
      .subscribe(val => this.navigateAndEmit(val));

    //subscription to query parameter changes
    this.queryParamsSubscription = this.router.routerState.root.queryParams.subscribe(
      (params: Params) => this.handleChangesToQueryParams(params)
    );

    this.selectedItemSubscription = this.selectedItemObs.subscribe(val => {
      this.selectedItem = val;
    });
  }

  ngOnDestroy() {
    //unsubscribe from changes
    this.formSubscription.unsubscribe();
    this.queryParamsSubscription.unsubscribe();
    this.selectedItemSubscription.unsubscribe();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      from: ['', Validators.required],
      to: ['', Validators.required],
      domain: [],
    });
  }

  private createFilterValues(values: any): FilterValues {
    let useSelectedValue =
      this.selectedItem !== null && this.selectedItem.item === values.domain;
    let domain = useSelectedValue ? values.domain.Name : values.domain;
    let domainId = useSelectedValue ? this.selectedItem.item.id : null;

    let filterValue = new FilterValues(
      new Date(values.from.year, values.from.month - 1, values.from.day),
      new Date(values.to.year, values.to.month - 1, values.to.day),
      domain,
      domainId,
      !StringUtils.isUndefinedNullOrWhitespace(domain) && domainId === null
    );

    return filterValue;
  }

  private navigateAndEmit(filterValues: FilterValues): void {
    let params =
      filterValues.domainId === null
        ? {
            beginDate: DateUtils.dateToString(filterValues.beginDate),
            endDate: DateUtils.dateToString(filterValues.endDate),
          }
        : {
            beginDate: DateUtils.dateToString(filterValues.beginDate),
            endDate: DateUtils.dateToString(filterValues.endDate),
            domainId: filterValues.domainId,
          };

    this.router.navigate(['/email-abuse'], { queryParams: params });

    this.filterValues.emit(filterValues);
  }

  private handleChangesToQueryParams(params: Params): void {
    if (this.calledHandleChangesToQueryParameters === false) {
      let beginDateParam = params['beginDate'];
      let endDateParam = params['endDate'];
      let domainParam = params['domain'];

      let beginDate = DateUtils.parseNgbDateStruct(beginDateParam);
      if (beginDate.parsed) {
        this.filterForm.controls['from'].setValue(beginDate.ngbDateStruct);
      }

      let endDate = DateUtils.parseNgbDateStruct(endDateParam);
      if (endDate.parsed) {
        this.filterForm.controls['to'].setValue(endDate.ngbDateStruct);
      }

      this.filterForm.controls['domain'].setValue(domainParam);

      if (beginDate.parsed === true || endDate.parsed) {
        this.calledHandleChangesToQueryParameters = true;
      }
    }
  }

  formatter = (x: { name: string; id: number }) => x.name;

  search = (text$: Observable<string>) =>
    text$
      .filter(val => val != undefined && val.length > 0)
      .debounceTime(300)
      .distinctUntilChanged()
      .switchMap(term =>
        this.domainSearchService.getMatchingDomains(trim(term)).catch(() => {
          return Observable.of([]);
        })
      );
}
