import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute  } from '@angular/router';
import { Subscription } from 'rxjs';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';
import { DmarcConfig } from '../../models/dmarc-config.model';

@Component({
  selector: 'dmarc',
  templateUrl: './dmarc.component.html',
  styleUrls: ['./dmarc.component.css']
})
export class DmarcComponent implements OnInit, OnDestroy {

 private _subscription : Subscription;
  
  public dmarcConfig : DmarcConfig;
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
          this.dmarcConfig = null;
          this.error = null;
          this.errored = false;
        })
        .map(params => +params['id'])
        .switchMap(id => this.dnsRecordInfoService.getDmarc(id))
        .subscribe(_ => {
          if(_.errored){
            this.errored = true;
            this.error = _.error;
          }
          else{
            this.errored = false;          
            this.dmarcConfig = _.value;
          }
          this.loading = false;
        });
  }

  ngOnDestroy(){
    this._subscription.unsubscribe();
  }
}
