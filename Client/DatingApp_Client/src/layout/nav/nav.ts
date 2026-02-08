import { Component, inject, signal } from '@angular/core';
import {FormsModule} from '@angular/forms'
import { AccountService } from '../../core/services/account-service';


@Component({
  selector: 'app-nav',
  standalone:true,
  imports: [FormsModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {

  protected accountService = inject(AccountService)
  // property to hold form inputs
  protected creds:any={}
  // protected loggedIn = signal(false)


  login(){
    // console.log(this.creds);
    this.accountService.login(this.creds).subscribe({
      next: result => {
        console.log(result);
        // this.loggedIn.set(true);
      },
      error: error => alert(error.message)
    })
  }

  logout(){
    this.accountService.logout();
  }
}
