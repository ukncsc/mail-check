import {
  Component,
  OnInit,
  Input,
  OnChanges,
  SimpleChanges
} from '@angular/core';

@Component({
  selector: 'pagination-display',
  templateUrl: './pagination-display.component.html',
  styleUrls: ['./pagination-display.component.css']
})
export class PaginationDisplayComponent implements OnChanges {
  @Input() page: number;
  @Input() pageSize: number;
  @Input() collectionSize: number;

  public minRecord: number = 0;
  public maxRecord: number = 0;
  public recordCount: number = 0;

  public ngOnChanges(changes: SimpleChanges) {
    this.minRecord = 0;
    this.maxRecord = 0;
    this.recordCount = 0;

    if (
      this.isDefined(this.page) &&
      this.isDefined(this.pageSize) &&
      this.isDefined(this.collectionSize) &&
      this.collectionSize > 0
    ) {
      let internalPage = Math.max(1, this.page);
      let internalPageSize = Math.max(1, this.pageSize);

      let potentialMinRecord = (internalPage - 1) * internalPageSize + 1;
      let potentialMaxRecord = potentialMinRecord + internalPageSize - 1;

      if (potentialMinRecord > this.collectionSize) {
        this.maxRecord = this.collectionSize;
        internalPage = Math.ceil(this.collectionSize / internalPageSize);
        this.minRecord = (internalPage - 1) * internalPageSize + 1;
      } else if (potentialMaxRecord > this.collectionSize) {
        this.maxRecord = this.collectionSize;
        this.minRecord = potentialMinRecord;
      } else {
        this.maxRecord = potentialMaxRecord;
        this.minRecord = potentialMinRecord;
      }
      this.recordCount = this.collectionSize;
    } else {
      this.minRecord = 0;
      this.maxRecord = 0;
      this.recordCount = 0;
    }
  }

  private isDefined(value: number): boolean {
    return value != null && value !== null && value !== undefined;
  }
}
