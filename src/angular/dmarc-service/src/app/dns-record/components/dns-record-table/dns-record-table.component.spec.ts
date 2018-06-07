// import { async, ComponentFixture, TestBed } from '@angular/core/testing';

// import { DnsRecordTableComponent } from './dns-record-table.component';
// import { DnsRecordModel } from '../../models/dns-record.model';
// import { DnsRecordsModel } from '../../models/dns-records.model';
// import { Domain } from '../../models/domain.model';
// import { MxRecord } from '../../models/mx-record.model';
// import { DmarcRecord } from '../../models/dmarc-record.model';
// import { SpfRecord } from '../../models/spf-record.model';
// import { By } from '@angular/platform-browser';

// describe('DnsRecordTableComponent', () => {
//   describe('Creation', () => {
//     let component: DnsRecordTableComponent;
//     let fixture: ComponentFixture<DnsRecordTableComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         declarations: [ DnsRecordTableComponent ]
//       })
//       .compileComponents();
//     }));

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordTableComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should be created', () => {
//       expect(component).toBeTruthy();
//     });
//   });

//   describe('Business Logic', () => {
//     let component: DnsRecordTableComponent;
//     let fixture: ComponentFixture<DnsRecordTableComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         declarations: [ DnsRecordTableComponent ]
//       })
//       .compileComponents();
//     }));

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordTableComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should find empty spfRecords invalid', () => {
//       let dnsRecordModel : DnsRecordModel = createDnsRecord();
//       let valid = component.isValidSpfRecord(dnsRecordModel);
//       expect(valid).toBe(false);
//     });

//     it('should find single spf record valid', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let spfRecords = [{
//         id : 1,
//         record : "v=spf.....",
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], spfRecords, domain);
//       let valid = component.isValidSpfRecord(dnsRecordModel);
//       expect(valid).toBe(true);
//     });

//     it('should find multiple spf records valid', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let spfRecords = [{
//         id : 1,
//         record : "v=spf.....",
//         lastChecked : new Date()
//        },
//        {
//         id : 1,
//         record : "v=spf.....",
//         lastChecked : new Date()
//        }]

//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], spfRecords, domain);
//       let valid = component.isValidSpfRecord(dnsRecordModel);
//       expect(valid).toBe(false);
//     });

//     it('should find empty dmarcRecords invalid', () => {
//       let dnsRecordModel : DnsRecordModel = createDnsRecord();
//       let valid = component.isValidDmarcRecord(dnsRecordModel);
//       expect(valid).toBe(false);
//     });

//     it('should find single dmarc record valid', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let dmarcRecords = [{
//         id : 1,
//         record : "v=dmarc.....",
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], dmarcRecords, [], domain);
//       let valid = component.isValidDmarcRecord(dnsRecordModel);
//       expect(valid).toBe(true);
//     });

//     it('should find multiple dmarc records valid', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let dmarcRecords = [{
//         id : 1,
//         record : "v=dmarc.....",
//         lastChecked : new Date()
//        },
//        {
//         id : 1,
//         record : "v=dmarc.....",
//         lastChecked : new Date()
//        }]

//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], dmarcRecords, domain);
//       let valid = component.isValidDmarcRecord(dnsRecordModel);
//       expect(valid).toBe(false);
//     });
//   })

//    describe('View Logic', () => {
//     let component: DnsRecordTableComponent;
//     let fixture: ComponentFixture<DnsRecordTableComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         declarations: [ DnsRecordTableComponent ]
//       })
//       .compileComponents();
//     }));

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordTableComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should render domain info correctly', () => {
//       let domain = {
//         id : 1,
//         name : 'abc.gov.uk'
//       };
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], [], domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let elements : HTMLElement[] = fixture.debugElement.queryAll(By.css('h6')).map(_ => _.nativeElement);

//       expect(elements.length).toBe(1);
//       expect(elements[0].textContent).toBe(domain.name);
//     });

//     it('should render empty dmarc records correctly', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let dmarcRecords = []
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], dmarcRecords, [], domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[2];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-danger']).toBe(true);
//       expect(paragraphs.length).toBe(0);
//     });

//     it('should render single dmarc records correctly', () => {
//       let domain = {
//         id : 1,
//         name : 'domain'
//       };

//       let dmarcRecords = [{
//         id : 1,
//         record : 'v=dmarc.....',
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], dmarcRecords, [], domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[2];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-success']).toBe(true);
//       expect(paragraphs.length).toBe(1);
//       expect((paragraphs[0].nativeElement as HTMLElement).textContent).toBe(dmarcRecords[0].record);
//     });

//     it('should render multiple dmarc records correctly', () => {
//       let domain = {
//         id : 1,
//         name : 'domain'
//       };

//       let dmarcRecords = [{
//         id : 1,
//         record : 'v=dmarc1.....',
//         lastChecked : new Date()
//        },
//        {
//         id : 2,
//         record : 'v=dmarc2.....',
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], dmarcRecords, [], domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[2];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-danger']).toBe(true);
//       expect(paragraphs.length).toBe(2);
//       expect((paragraphs[0].nativeElement as HTMLElement).textContent).toBe(dmarcRecords[0].record);
//       expect((paragraphs[1].nativeElement as HTMLElement).textContent).toBe(dmarcRecords[1].record);
//     });

//     it('should render empty spf records correctly', () => {
//       let domain = {
//         id : 1,
//         name : "domain"
//       };

//       let spfRecords = []
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], spfRecords, domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[3];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-danger']).toBe(true);
//       expect(paragraphs.length).toBe(0);
//     });

//     it('should render single spf records correctly', () => {
//       let domain = {
//         id : 1,
//         name : 'domain'
//       };

//       let spfRecords = [{
//         id : 1,
//         record : 'v=spf.....',
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], spfRecords, domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[3];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-success']).toBe(true);
//       expect(paragraphs.length).toBe(1);
//       expect((paragraphs[0].nativeElement as HTMLElement).textContent).toBe(spfRecords[0].record);
//     });

//     it('should render multiple spf records correctly', () => {
//       let domain = {
//         id : 1,
//         name : 'domain'
//       };

//       let spfRecords = [{
//         id : 1,
//         record : 'v=spf1.....',
//         lastChecked : new Date()
//        },
//        {
//         id : 2,
//         record : 'v=spf2.....',
//         lastChecked : new Date()
//        }]
//       let dnsRecordModel : DnsRecordModel = createDnsRecord([], [], spfRecords, domain);
//       let dnsRecordsModel : DnsRecordsModel = createDnsRecords([dnsRecordModel]);

//       component.dnsRecordsModel = dnsRecordsModel;
//       fixture.detectChanges();

//       let tableCell = fixture.debugElement
//         .queryAll(By.css('td'))[3];

//       let paragraphs = tableCell
//         .queryAll(By.css('p'));

//       expect(tableCell.classes['table-danger']).toBe(true);
//       expect(paragraphs.length).toBe(2);
//       expect((paragraphs[0].nativeElement as HTMLElement).textContent).toBe(spfRecords[0].record);
//       expect((paragraphs[1].nativeElement as HTMLElement).textContent).toBe(spfRecords[1].record);
//     });

//   });
// });

// function createDnsRecords(dnsRecords : DnsRecordModel[]) : DnsRecordsModel{
//   let dnsRecordsModel : DnsRecordsModel = {
//     domains : dnsRecords,
//     domainCount : 1,
//     page : 1,
//     pageSize : 1,
//     search : undefined
//   }

//   return dnsRecordsModel;
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
