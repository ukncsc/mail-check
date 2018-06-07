import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';
import { Domain } from '../../models/domain.model';

@Component({
  selector: 'domain-security-header',
  templateUrl: './domain-security-header.component.html',
  styleUrls: ['./domain-security-header.component.css']
})
export class DomainSecurityHeaderComponent implements OnInit, OnDestroy {
  private _subscription: Subscription;

  public domain: Domain;
  public error: Error;
  public errored: boolean;
  public loading: boolean;

  constructor(
    private route: ActivatedRoute,
    private dnsRecordInfoService: DnsRecordInfoService
  ) {}

  ngOnInit() {
    this._subscription = this.route.params
      .do(() => {
        this.loading = true;
        this.domain = null;
        this.error = null;
        this.errored = false;
      })
      .map(params => +params['id'])
      .switchMap(id => this.dnsRecordInfoService.getDomainById(id))
      .subscribe(_ => {
        if (_.errored) {
          this.errored = true;
          this.error = _.error;
        } else {
          this.errored = false;
          this.domain = _.value;
        }
        this.loading = false;
      });
  }

  ngOnDestroy() {
    this._subscription.unsubscribe();
  }
}
