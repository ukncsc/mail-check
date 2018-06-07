import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute  } from '@angular/router';
import { Subscription } from 'rxjs';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';
import { ReceivingEncrypted } from '../../models/receiving_encrypted.model';

@Component({
  selector: 'tls',
  templateUrl: './tls.component.html',
  styleUrls: ['./tls.component.css']
})
export class TlsComponent implements OnInit , OnDestroy {

private _subscription : Subscription;
  
  public receivingEncrypted : ReceivingEncrypted;
  public error : Error;
  public errored : boolean;
  public loading : boolean;
  public isCollapsed = true;

  constructor(private route : ActivatedRoute,
    private dnsRecordInfoService : DnsRecordInfoService) { 
  }

  ngOnInit() {
       this._subscription = this.route.params
        .do(() => {
          this.loading = true;
          this.receivingEncrypted = null;
          this.error = null;
          this.errored = false;
        })
        .map(params => +params['id'])
        .switchMap(id => this.dnsRecordInfoService.getReceivingEncryptedById(id))
        .subscribe(_ => {
          if(_.errored){
            this.errored = true;
            this.error = _.error;
          }
          else{
            this.errored = false;          
            this.receivingEncrypted = _.value;
          }
          this.loading = false;
        });
  }

  ngOnDestroy(){
    this._subscription.unsubscribe();
  }
}
