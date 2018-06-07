// import { async, ComponentFixture, TestBed } from '@angular/core/testing';
// import { Component, ViewChild } from '@angular/core';
// import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
// import { StatusControlComponent } from './status-control.component';
// import { StatusControlBaseComponent } from '../status-control-base/status-control-base.component';
// import { LoaderComponent } from '../loader/loader.component';
// import { NoResultsComponent } from '../no-results/no-results.component';
// import { ErrorComponent } from '../error/error.component';
// import { Status } from '../status-control-base/status.enum';
// import { By } from '@angular/platform-browser';

// describe('StatusControlComponent', () => {
//     describe('Creation', () => {
//         let component: StatusControlComponent;
//         let fixture: ComponentFixture<StatusControlComponent>;

//         beforeEach(async(() => {
//             TestBed.configureTestingModule({
//             imports: [ NgbModule ],
//             declarations: [
//             StatusControlComponent,
//             LoaderComponent,
//             StatusControlBaseComponent,
//             ErrorComponent,
//             NoResultsComponent ]
//             })
//             .compileComponents();
//         }));

//         beforeEach(() => {
//             fixture = TestBed.createComponent(StatusControlComponent);
//             component = fixture.componentInstance;
//             fixture.detectChanges();
//         });

//         it('should be created', () => {
//             expect(component).toBeTruthy();
//         });
//     });

//     describe('View Logic', () => {
//         let component: StatusControlComponentHost;
//         let fixture: ComponentFixture<StatusControlComponentHost>;

//          beforeEach(async(() => {
//             TestBed.configureTestingModule({
//             imports: [ NgbModule ],
//             declarations: [
//             StatusControlComponent,
//             LoaderComponent,
//             StatusControlBaseComponent,
//             ErrorComponent,
//             NoResultsComponent,
//             StatusControlComponentHost ]
//             })
//             .compileComponents();
//         }));

//         beforeEach(() => {
//             fixture = TestBed.createComponent(StatusControlComponentHost);
//             component = fixture.componentInstance;
//             fixture.detectChanges();
//         });

//         it('displays no data when status is NoData', () => {
//             component.statusControlComponent.status = Status.NoData;

//             fixture.detectChanges();

//             let element : HTMLElement = fixture.debugElement.query(By.css('h4')).nativeElement;

//             expect(element.textContent).toBe('The search returned no results.');
//         });

//         it('displays error when status is Error', () => {
//             component.statusControlComponent.status = Status.Error;

//             fixture.detectChanges();

//             let element : HTMLElement = fixture.debugElement.query(By.css('h5')).nativeElement;

//             expect(element.textContent).toBe('Error:');
//         });

//         it('displays loader when status is Loading', () => {
//             component.statusControlComponent.status = Status.Loading;

//             fixture.detectChanges();

//             let element : HTMLElement = fixture.debugElement.query(By.css('p')).nativeElement;

//             expect(element.textContent).toBe('Loading...');
//         });

//         it('displays content when status is Loaded', () => {
//             component.statusControlComponent.status = Status.Loaded;

//             fixture.detectChanges();

//             let element : HTMLElement = fixture.debugElement.query(By.css('p')).nativeElement;

//             expect(element.textContent).toBe('Test');
//         });
//     });
// });

// @Component({
//   template:'<status-control><p>Test</p></status-control>'
// })
// class StatusControlComponentHost{
//     @ViewChild(StatusControlComponent) statusControlComponent:StatusControlComponent;
// }
