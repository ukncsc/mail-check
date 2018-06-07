import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute  } from '@angular/router';
import { Subscription } from 'rxjs';
import { DnsRecordInfoService } from '../../services/dns-record-info.service';
import { SpfConfig } from '../../models/spf-config.model';

@Component({
  selector: 'spf',
  templateUrl: './spf.component.html',
  styleUrls: ['./spf.component.css']
})
export class SpfComponent implements OnInit, OnDestroy {

  private _subscription : Subscription;
  
  public spfConfig : SpfConfig;
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
          this.spfConfig = null;
          this.error = null;
          this.errored = false;
        })
        .map(params => +params['id'])
        .switchMap(id => this.dnsRecordInfoService.getSpf(id))
        .subscribe(_ => {
          if(_.errored){
            this.errored = true;
            this.error = _.error;
          }
          else{
            this.errored = false;          
            this.spfConfig = _.value;
          }
          this.loading = false;
        });
  }

  ngOnDestroy(){
    this._subscription.unsubscribe();
  }

}
