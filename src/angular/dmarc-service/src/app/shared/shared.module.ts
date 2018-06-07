import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LargeNumberPipe } from './pipes/large-number.pipe';

@NgModule({
  imports: [CommonModule],
  declarations: [LargeNumberPipe],
  exports: [LargeNumberPipe]
})
export class SharedModule {}
