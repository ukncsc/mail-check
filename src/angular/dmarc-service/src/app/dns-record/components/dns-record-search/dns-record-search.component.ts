import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {
  FormControl,
  Validators,
  FormGroup,
  FormBuilder
} from '@angular/forms';
import { Subscription, Observable, Subject } from 'rxjs';

@Component({
  selector: 'dns-record-search',
  templateUrl: './dns-record-search.component.html',
  styleUrls: ['./dns-record-search.component.css']
})
export class DnsRecordSearchComponent implements OnInit {
  @Input() search: string;
  @Output() searchChange: EventEmitter<string> = new EventEmitter();

  public searchForm: FormGroup;
  private searchFormSub: Subscription;

  constructor(private formBuilder: FormBuilder) {}

  public ngOnInit() {
    this.searchForm = this.formBuilder.group({ search: '' });

    this.searchFormSub = this.searchForm.valueChanges
      .debounceTime(300)
      .distinctUntilChanged()
      .subscribe(val => {
        this.search = val.search;
        this.searchChange.next(this.search);
      });
  }
}
