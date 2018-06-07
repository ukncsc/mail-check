// import { async, ComponentFixture, TestBed } from '@angular/core/testing';
// import { PaginationDisplayComponent } from './pagination-display.component';
// import { By } from '@angular/platform-browser';
// import { DebugElement } from '@angular/core';

// describe('PaginationDisplayComponent', () => {
//   describe('Creation', () => {
//     let component: PaginationDisplayComponent;
//     let fixture: ComponentFixture<PaginationDisplayComponent>;

//     beforeEach(async(() => {
//       TestBed.configureTestingModule({
//         declarations: [ PaginationDisplayComponent ]
//       })
//       .compileComponents();
//     }));

//     beforeEach(() => {
//       fixture = TestBed.createComponent(PaginationDisplayComponent);
//       component = fixture.componentInstance;
//       fixture.detectChanges();
//     });

//     it('should be created', () => {
//       expect(component).toBeTruthy();
//     });
//   });

//   describe('Business Logic', () => {
//     let component: PaginationDisplayComponent;
//     let fixture: ComponentFixture<PaginationDisplayComponent>;

//     beforeEach(() => {
//       component = new PaginationDisplayComponent();
//     });

//     it('should calculate correct value for minRecord', () => {
//       component.page = 1
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);
//       expect(component.minRecord).toEqual(1);

//       component.page = 2
//       component.ngOnChanges(null);
//       expect(component.minRecord).toEqual(51);

//       component.page = 10;
//       component.collectionSize = 499;
//       component.ngOnChanges(null);
//       expect(component.minRecord).toEqual(451);

//       component.page = 11
//       component.ngOnChanges(null);
//       expect(component.minRecord).toEqual(451);
//     });

//     it('should calculate correct value for maxRecord', () => {
//       component.page = 1
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);
//       expect(component.maxRecord).toEqual(50);

//       component.page = 2
//       component.ngOnChanges(null);
//       expect(component.maxRecord).toEqual(100);

//       component.page = 10;
//       component.collectionSize = 499;
//       component.ngOnChanges(null);
//       expect(component.maxRecord).toEqual(499);

//       component.page = 11;
//       component.ngOnChanges(null);
//       expect(component.maxRecord).toEqual(499);
//     });

//     it('should calculate correct value for recordCount', () => {
//       component.page = 1
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);
//       expect(component.recordCount).toEqual(500);

//       component.collectionSize = 1000
//       component.ngOnChanges(null);
//       expect(component.recordCount).toEqual(1000);
//     });

//     it('should calculate minRecord to be 0 when inputs are undefined or null', () => {
//       expect(component.minRecord).toBe(0);

//       component.page = null;
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);

//       expect(component.minRecord).toBe(0);

//       component.page = 1;
//       component.pageSize = null;
//       component.ngOnChanges(null);

//       expect(component.minRecord).toBe(0);

//       component.pageSize = 50;
//       component.collectionSize = null;
//       component.ngOnChanges(null);

//       expect(component.minRecord).toBe(0);

//       component.collectionSize = -1;
//       component.ngOnChanges(null);

//       expect(component.minRecord).toBe(0);
//     })

//     it('should calculate maxRecord to be 0 when inputs are undefined or null', () => {
//       expect(component.maxRecord).toBe(0);

//       component.page = null;
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);

//       expect(component.maxRecord).toBe(0);

//       component.page = 1;
//       component.pageSize = null;
//       component.ngOnChanges(null);

//       expect(component.maxRecord).toBe(0);

//       component.pageSize = 50;
//       component.collectionSize = null;
//       component.ngOnChanges(null);

//       expect(component.maxRecord).toBe(0);

//       component.collectionSize = -1;
//       component.ngOnChanges(null);

//       expect(component.maxRecord).toBe(0);
//     })

//     it('should calculate recordCount to be 0 when inputs are undefined or null', () => {
//       expect(component.recordCount).toBe(0);

//       component.page = null;
//       component.pageSize = 50;
//       component.collectionSize = 500;
//       component.ngOnChanges(null);

//       expect(component.recordCount).toBe(0);

//       component.page = 1;
//       component.pageSize = null;
//       component.ngOnChanges(null);

//       expect(component.recordCount).toBe(0);

//       component.pageSize = 50;
//       component.collectionSize = null;
//       component.ngOnChanges(null);

//       expect(component.recordCount).toBe(0);

//       component.collectionSize = -1;
//       component.ngOnChanges(null);

//       expect(component.recordCount).toBe(0);
//     })
//   });

//   describe('PaginationDisplayComponent Display Logic', () => {
//     let component: PaginationDisplayComponent;
//     let fixture: ComponentFixture<PaginationDisplayComponent>;

//     beforeEach(() => {
//       TestBed.configureTestingModule({
//         declarations: [ PaginationDisplayComponent ]
//       });

//       fixture = TestBed.createComponent(PaginationDisplayComponent);
//       component = fixture.componentInstance;
//     });

//     it('should render correct values', () => {
//       let p : HTMLElement = fixture.debugElement.query(By.css('p')).nativeElement;

//       component.page = 1
//       component.pageSize = 50;
//       component.collectionSize = 500;

//       component.ngOnChanges(null);

//       fixture.detectChanges();

//       expect(p.textContent).toBe('Showing 1-50 of 500 domains');
//     });
//   });
// });
