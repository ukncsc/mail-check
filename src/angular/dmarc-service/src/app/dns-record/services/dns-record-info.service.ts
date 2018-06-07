import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Injectable } from "@angular/core";
import { Observable } from 'rxjs/Rx';
import { DomainsSecurityInfo} from '../models/domains-security-info.model';
import { Domain } from '../models/domain.model';
import { AntiSpoofing } from '../models/anti-spoofing.model';
import { ReceivingEncrypted } from '../models/receiving_encrypted.model';
import { SpfConfig } from '../models/spf-config.model';
import { DmarcConfig } from '../models/dmarc-config.model';

@Injectable()
export class DnsRecordInfoService{

    private url;

    private headers = new Headers({'Accept': 'application/json'});

    constructor(private http: Http) {
        this.url = `${window.location.origin}/api/domainstatus`;
        //this.url = `http://localhost:5002/api/domainstatus`;
    }

    public getDomainsSecurityInfo(page : number, pageSize : number, search: string) : Observable<Result<DomainsSecurityInfo>>{
        let urlSearchParams = new URLSearchParams();
        urlSearchParams.append(`page`, page.toString());
        urlSearchParams.append(`pageSize`, pageSize.toString());
        if(search !== null){
            urlSearchParams.append('search', search);
        }

        let options = new RequestOptions({ headers: this.headers, search: urlSearchParams });

        return this.http
            .get(`${this.url}/domains_security`, options)
            .map(response => new Result<DomainsSecurityInfo>(response.json(), false, null))
            .catch(error => Observable.of(new Result<DomainsSecurityInfo>(null, true, error)));
    }

    public getDomainById(domainId : number) : Observable<Result<Domain>>{
        let options = new RequestOptions({ headers: this.headers });

        return this.http
            .get(`${this.url}/domain/${domainId.toString()}`, options)
            .map(response => new Result<Domain>(response.json(), false, null))
            .catch(error => Observable.of(new Result<Domain>(null, true, error)));
    }

    public getAntiSpoofingById(domainId : number) : Observable<Result<AntiSpoofing>>{
    let options = new RequestOptions({ headers: this.headers });

        return this.http
            .get(`${this.url}/domain/anti_spoofing/${domainId.toString()}`, options)
            .map(response => new Result<AntiSpoofing>(response.json(), false, null))
            .catch(error => Observable.of(new Result<AntiSpoofing>(null, true, error)));
    }

    public getReceivingEncryptedById(domainId : number) : Observable<Result<ReceivingEncrypted>>{
        let options = new RequestOptions({ headers: this.headers});

        return this.http.get(`${this.url}/domain/receiving_encrypted/${domainId.toString()}`, options)
            .map(response => new Result<ReceivingEncrypted>(response.json(), false, null))
            .catch(error => Observable.of(new Result<ReceivingEncrypted>(null, true, error)));
    }

    public getSpf(domainId : number) : Observable<Result<SpfConfig>>{
        let options = new RequestOptions({ headers: this.headers});

        return this.http.get(`${this.url}/domain/spf/${domainId.toString()}`, options)
            .map(response => new Result<SpfConfig>(response.json(), false, null))
            .catch(error => Observable.of(new Result<SpfConfig>(null, true, error)));
    }

    public getDmarc(domainId : number) : Observable<Result<DmarcConfig>>{
        let options = new RequestOptions({ headers: this.headers});

        return this.http.get(`${this.url}/domain/dmarc/${domainId.toString()}`, options)
            .map(response => new Result<DmarcConfig>(response.json(), false, null))
            .catch(error => Observable.of(new Result<DmarcConfig>(null, true, error)));
    }
}

export class Result<T>{
    
    constructor(value : T,
        errored : boolean,
        error : Error) {
            this.value = value;
            this.errored = errored;
            this.error = error;
    }

    value : T;
    errored : boolean;
    error : Error;
}