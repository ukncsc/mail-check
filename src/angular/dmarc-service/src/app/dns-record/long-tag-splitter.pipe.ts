import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'longTagSplitter'
})
export class LongTagSplitterPipe implements PipeTransform {

  transform(value: string): any {
    return value.match(/[^,]+,?|,/g);
  }
}
