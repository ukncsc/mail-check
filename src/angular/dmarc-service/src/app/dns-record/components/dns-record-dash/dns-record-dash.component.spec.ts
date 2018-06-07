// import { fakeAsync, tick, async, ComponentFixture, TestBed, inject, discardPeriodicTasks } from '@angular/core/testing';
// import { By } from '@angular/platform-browser';
// import { provideRoutes, Router, ActivatedRoute, Params, NavigationExtras } from '@angular/router';
// import { RouterModule } from '@angular/router';
// import { RouterTestingModule } from '@angular/router/testing';
// import { NgModule, Injectable } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
// import { DnsRecordDashComponent } from '../dns-record-dash/dns-record-dash.component';
// import { DnsRecordInfoService} from '../../services/dns-record-info.service';
// import { PaginationDisplayComponent } from '..//pagination-display/pagination-display.component';
// import { DnsRecordTableComponent } from '../dns-record-table/dns-record-table.component';
// import { StatusControlComponent } from '../status-control/status-control.component';
// import { DnsRecordSearchComponent } from '../dns-record-search/dns-record-search.component';
// import { LoaderComponent } from '../loader/loader.component';
// import { StatusControlBaseComponent } from '../status-control-base/status-control-base.component';
// import { ErrorComponent } from '../error/error.component';
// import { NoResultsComponent } from '../no-results/no-results.component';
// import { Observable, Observer, Subject, BehaviorSubject } from 'rxjs/Rx';
// import { routes } from '../../../app.routing';
// import { Location } from '@angular/common';
// import { Component } from '@angular/core';
// import { AggregateReportModule } from '../../../aggregate-report/aggregate-report.module';
// import { Status } from '../status-control-base/status.enum';
// import { PageService, } from '../../services/page.service';
// import { SearchService, } from '../../services/search.service';
// import { DnsRecordModel } from '../../models/dns-record.model';
// import { Domain } from '../../models/domain.model';
// import { MxRecord } from '../../models/mx-record.model';
// import { DmarcRecord } from '../../models/dmarc-record.model';
// import { SpfRecord } from '../../models/spf-record.model';

// describe('DnsRecordDash', () => {
//   describe('Creation', () => {
//     let component: DnsRecordDashComponent;
//     let fixture: ComponentFixture<DnsRecordDashComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         imports: [
//           CommonModule,
//           FormsModule,
//           ReactiveFormsModule,
//           AggregateReportModule,
//           RouterTestingModule.withRoutes(routes),
//           NgbModule.forRoot()
//         ],
//         declarations: [
//           DnsRecordDashComponent,
//           PaginationDisplayComponent,
//           DnsRecordTableComponent,
//           StatusControlComponent,
//           DnsRecordSearchComponent,
//           LoaderComponent,
//           StatusControlBaseComponent,
//           ErrorComponent,
//           NoResultsComponent
//         ],
//         providers: [
//           DnsRecordInfoService,
//           PageService,
//           SearchService
//         ],
//       })
//       .compileComponents();
//     }));

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordDashComponent);

//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should be created', () => {
//       expect(component).toBeTruthy();
//     });
//   });

//   describe('Business Logic', () => {
//     let component: DnsRecordDashComponent;
//     let fixture: ComponentFixture<DnsRecordDashComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         imports: [
//           CommonModule,
//           FormsModule,
//           AggregateReportModule,
//           ReactiveFormsModule,
//           RouterModule,
//           RouterTestingModule.withRoutes(routes),
//           NgbModule.forRoot()
//         ],
//         declarations: [
//           DnsRecordDashComponent,
//           PaginationDisplayComponent,
//           DnsRecordTableComponent,
//           StatusControlComponent,
//           DnsRecordSearchComponent,
//           LoaderComponent,
//           StatusControlBaseComponent,
//           ErrorComponent,
//           NoResultsComponent
//         ],
//          providers: [
//           DnsRecordInfoService,
//           PageService,
//           SearchService,
//           { provide: ActivatedRoute, useValue: { queryParams : new Subject<Params>() }},
//         ],
//       });
//     }));

//     beforeEach(()=> {
//       fixture = TestBed.createComponent(DnsRecordDashComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should navigate to default page if none set',
//       fakeAsync(inject([Router, ActivatedRoute, DnsRecordInfoService], (router, route, dnsRecordInfoService) => {
//         let search = undefined;
//         let page = 1;
//         let pageSize = 25;

//         spyOn(router, 'navigate');
//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//         route.queryParams.next({pageSize : pageSize});

//         tick(300);

//         expect(router.navigate).toHaveBeenCalledTimes(1);
//         expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: page, pageSize: pageSize}});
//     })));

//     it('should navigate with default pageSize if none set',
//      fakeAsync(inject([Router, ActivatedRoute, DnsRecordInfoService], (router, route, dnsRecordInfoService) => {
//         let search = undefined;
//         let page = 2;
//         let pageSize = 50;

//         spyOn(router, 'navigate');
//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//         route.queryParams.next({page : page});

//         tick(300);

//         expect(router.navigate).toHaveBeenCalledTimes(1);
//         expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: page, pageSize: pageSize}});
//     })));

//     it('should navigate to correct page',
//       fakeAsync(inject([Router, PageService, DnsRecordInfoService], (router, pageService, dnsRecordInfoService) => {
//         let search = undefined;
//         let page = 5;
//         let pageSize = 50;

//         spyOn(router, 'navigate');
//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//         pageService.page.next(page);

//         tick(300);

//         expect(router.navigate).toHaveBeenCalledTimes(1);
//         expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: page, pageSize: pageSize}});
//     })));

//     it('should navigate with correct search',
//       fakeAsync(inject([Router, SearchService, DnsRecordInfoService], (router, searchService, dnsRecordInfoService) => {
//         let search = 'search';
//         let page = 1;
//         let pageSize = 50;

//         spyOn(router, 'navigate');
//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//         searchService.search.next(search);

//         tick(300);

//         expect(router.navigate).toHaveBeenCalledTimes(1);
//         expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: page, pageSize: pageSize, search: search}});
//     })));

//     it('should have status of no data on start up', fakeAsync(() => {
//       expect(component.status).toBe(Status.NoData);
//     }));

//     it('should have status of loading when fetching data',
//       fakeAsync(inject([ActivatedRoute, DnsRecordInfoService], (route, dnsRecordInfoService) => {
//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue( Observable.never());

//         route.queryParams.next({page : 1, pageSize : 50});

//         tick(300);

//         expect(component.status).toBe(Status.Loading);
//     })));

//     it('should have status of loaded when data successfully fetched',
//       fakeAsync(inject([ActivatedRoute, DnsRecordInfoService], (route, dnsRecordInfoService) => {
//         let search = undefined;
//         let page = 1;
//         let pageSize = 50;

//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//         route.queryParams.next({page : page, pageSize : pageSize});

//         tick(300);

//         expect(component.status).toBe(Status.Loaded);
//     })));

//      it('should have status of no data when no results returned',
//       fakeAsync(inject([ActivatedRoute, DnsRecordInfoService], (route, dnsRecordInfoService) => {
//         let search = undefined;
//         let page = 1;
//         let pageSize = 50;

//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(0, page, pageSize, search));

//         route.queryParams.next({page : page, pageSize : pageSize});

//         tick(300);

//         expect(component.status).toBe(Status.NoData);
//     })));

//     it('should have status of error when data fetched with error',
//        fakeAsync(inject([ActivatedRoute, DnsRecordInfoService], (route, dnsRecordInfoService) => {

//         spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(Observable.throw('Error occurred'));

//         route.queryParams.next({page : 1, pageSize : 50});

//         tick(300);

//         expect(component.status).toBe(Status.Error);
//       })));

//       it('should call dnsInfoSevice when valid parameters provided',
//         fakeAsync(inject([Router, ActivatedRoute, DnsRecordInfoService], (router, route, dnsRecordInfoService) => {
//           let search = undefined;
//           let page = 1;
//           let pageSize = 50;

//           spyOn(router, 'navigate');
//           spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//           route.queryParams.next({page : page, pageSize : pageSize});

//           tick(300);

//           expect(dnsRecordInfoService.getDnsInfo).toHaveBeenCalledTimes(1);
//           expect(dnsRecordInfoService.getDnsInfo).toHaveBeenCalledWith(page, pageSize, undefined);
//       })));

//       it('should reset page on new search parameter',
//         fakeAsync(inject([Router, ActivatedRoute, SearchService, DnsRecordInfoService], (router, route, searchService, dnsRecordInfoService) => {
//           let search = 'search';
//           let pageSize = 50;

//           spyOn(router, 'navigate');
//           spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, 1, pageSize, search));

//           route.queryParams.next({page : 2, pageSize : 50});

//           searchService.search.next(search);

//           tick(600);

//           expect(router.navigate).toHaveBeenCalledTimes(1);
//         expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: 1, pageSize: 50, search: search}});
//       })));

//       it('should respect page size on new search parameter',
//         fakeAsync(inject([Router, ActivatedRoute, SearchService, DnsRecordInfoService], (router, route, searchService, dnsRecordInfoService) => {

//           let search = 'search';
//           let page = 1;
//           let pageSize = 25;

//           spyOn(router, 'navigate');
//           spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search));

//           route.queryParams.next({page : page, pageSize : pageSize});

//           searchService.search.next(search);

//           tick(600);

//           expect(router.navigate).toHaveBeenCalledTimes(1);
//           expect(router.navigate).toHaveBeenCalledWith(['/dns-record-dash'], {queryParams : {page: page, pageSize: pageSize, search: search}});
//       })));

//       it('should update fields on successful call to api',
//         fakeAsync(inject([Router, ActivatedRoute, SearchService, DnsRecordInfoService], (router, route, searchService, dnsRecordInfoService) => {
//           spyOn(router, 'navigate');

//           let search =  'search';
//           let page = 1;
//           let pageSize = 20;

//           let domain = {
//             id : 1,
//             name : "domain"
//           };

//           let dnsInfo = [createDnsRecord([], [], [], domain)]

//           spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(createDnsInfo(1, page, pageSize, search, dnsInfo));

//           route.queryParams.next({page : page, pageSize : pageSize, search : search});

//           tick(300);

//           expect(component.status).toBe(Status.Loaded);
//           expect(component.dnsRecordsModel.domains).toBe(dnsInfo);
//           expect(component.search).toBe(search);
//           expect(component.pageSize).toBe(pageSize);
//           expect(component.page).toBe(page);
//           expect(component.errorMessage).toBe('');
//       })));

//        it('should update error fields on failed call to api',
//         fakeAsync(inject([Router, ActivatedRoute, SearchService, DnsRecordInfoService], (router, route, searchService, dnsRecordInfoService) => {
//           spyOn(router, 'navigate');

//           let search =  'search';
//           let page = 1;
//           let pageSize = 20;

//           let errorMessage = 'error message';

//           spyOn(dnsRecordInfoService, 'getDnsInfo').and.returnValue(Observable.throw(new Error()));

//           route.queryParams.next({page : page, pageSize : pageSize, search : search});

//           tick(300);

//           expect(component.status).toBe(Status.Error);
//           expect(component.errorMessage).toBe('Error');
//       })));
//   });
// });

// function createDnsInfo(domainCount : number, page: number, pageSize : number, search : string, dnsRecords : DnsRecordModel[] = []) : Observable<any> {
//   return Observable.of({
//     domains : dnsRecords,
//     domainCount: domainCount,
//     page: page,
//     pageSize: pageSize,
//     search: search
//   });
// }

// function createDnsRecord(
//   mxRecords : MxRecord[] = [],
//   dmarcRecords : DmarcRecord[] = [],
//   spfRecords : SpfRecord[] = [],
//   domain : Domain = undefined) : DnsRecordModel{

//     let dnsRecordModel : DnsRecordModel = {
//       domain : domain,
//       hasNsRecords : true,
//       mxRecords : mxRecords,
//       dmarcRecords : dmarcRecords,
//       spfRecords : spfRecords
//     };

//     return dnsRecordModel;
// }
