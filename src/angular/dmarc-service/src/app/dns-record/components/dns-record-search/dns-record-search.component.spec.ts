// import {
//   fakeAsync,
//   tick,
//   async,
//   ComponentFixture,
//   TestBed
// } from '@angular/core/testing';
// import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { By } from '@angular/platform-browser';

// import { DnsRecordSearchComponent } from './dns-record-search.component';

// describe('DnsRecordSearchComponent', () => {
//   describe('Creation', () => {
//     let component: DnsRecordSearchComponent;
//     let fixture: ComponentFixture<DnsRecordSearchComponent>;

//     beforeEach(
//       async(() => {
//         TestBed.configureTestingModule({
//           imports: [FormsModule, ReactiveFormsModule],
//           declarations: [DnsRecordSearchComponent]
//         }).compileComponents();
//       })
//     );

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordSearchComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should be created', () => {
//       expect(component).toBeTruthy();
//     });
//   });

//   describe('Business Logic', () => {
//     let component: DnsRecordSearchComponent;
//     let fixture: ComponentFixture<DnsRecordSearchComponent>;

//     beforeEach(
//       async(() => {
//         TestBed.configureTestingModule({
//           imports: [FormsModule, ReactiveFormsModule],
//           declarations: [DnsRecordSearchComponent]
//         }).compileComponents();
//       })
//     );

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordSearchComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it(
//       'should raise changes after 300ms after form values updated',
//       fakeAsync(() => {
//         let value = 'test value';
//         component.searchForm.controls['search'].setValue(value);
//         component.searchChange.subscribe(_ => {
//           expect(_).toBe(value);
//         });
//         expect(component.search).toBe(undefined);
//         tick(300);
//         expect(component.search).toBe(value);
//       })
//     );

//     it(
//       'should not raise changes on duplicate updates',
//       fakeAsync(() => {
//         let value = 'test value';
//         let count = 0;
//         component.searchForm.controls['search'].setValue(value);
//         component.searchForm.controls['search'].setValue(value);
//         component.searchChange.subscribe(_ => {
//           count++;
//         });
//         expect(count).toBe(0);
//         tick(300);
//         expect(component.search).toBe(value);
//         expect(count).toBe(1);
//       })
//     );
//   });

//   describe('Display Logic', () => {
//     let component: DnsRecordSearchComponent;
//     let fixture: ComponentFixture<DnsRecordSearchComponent>;

//     beforeEach(
//       async(() => {
//         TestBed.configureTestingModule({
//           imports: [FormsModule, ReactiveFormsModule],
//           declarations: [DnsRecordSearchComponent]
//         }).compileComponents();
//       })
//     );

//     beforeEach(() => {
//       fixture = TestBed.createComponent(DnsRecordSearchComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it(
//       'should render updates to serach values',
//       fakeAsync(() => {
//         let input: HTMLInputElement = fixture.debugElement.query(
//           By.css('input')
//         ).nativeElement;
//         let value = 'search value';

//         component.search = value;

//         fixture.detectChanges();

//         expect(input.value).toBe(value);
//       })
//     );
//   });
// });
