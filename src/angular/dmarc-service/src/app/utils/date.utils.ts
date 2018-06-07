import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { StringUtils } from '../utils/string.utils';
import { DatePipe } from '@angular/common';
import * as moment from 'moment';

export class DateUtils {
  public static dateToString(date: Date): string {
    return moment(date).format('YYYY-MM-DD');
  }

  public static formatDateString(date: string): string {
    // Slice the time off the date
    return date.slice(0, 10);
  }

  public static parseDate(str: string): { parsed: boolean; date: Date } {
    let beginDateNum: number = Date.parse(str);
    if (!isNaN(beginDateNum)) {
      return { parsed: true, date: new Date(beginDateNum) };
    }
    return { parsed: false, date: null };
  }

  public static dateToNgbDateStruct(date: Date): NgbDateStruct {
    return {
      day: date.getUTCDate(),
      month: date.getUTCMonth() + 1,
      year: date.getUTCFullYear()
    };
  }

  public static parseNgbDateStruct(
    str: string
  ): { parsed: boolean; ngbDateStruct: NgbDateStruct } {
    if (!StringUtils.isUndefinedNullOrWhitespace(str)) {
      let result = DateUtils.parseDate(str);
      if (result.parsed) {
        return {
          parsed: true,
          ngbDateStruct: this.dateToNgbDateStruct(result.date)
        };
      }
    }
    return { parsed: false, ngbDateStruct: null };
  }
}
