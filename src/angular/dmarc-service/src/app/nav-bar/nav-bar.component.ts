import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../user.service';
import { subscribeOn } from 'rxjs/operator/subscribeOn';
import { Subscription } from 'rxjs';

@Component({
  selector: 'nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit, OnDestroy {

  public isAdmin : boolean = false;
  public subscription : Subscription;

  constructor(private userService : UserService) { }

  ngOnInit() {
    this.subscription = this.userService.isAdmin().subscribe((isAdmin) => {
      this.isAdmin = isAdmin;
    })
  }

  ngOnDestroy(){
    this.subscription.unsubscribe();
  }
}
