import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'largenumber' })
export class LargeNumberPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    let numberValue = Number(value);
    if (numberValue >= 1000000) {
      return `${(numberValue / 1000000).toFixed(2)}M`;
    } else if (numberValue > 1000) {
      return `${(numberValue / 1000).toFixed(2)}K`;
    }
    return numberValue;
  }
}
