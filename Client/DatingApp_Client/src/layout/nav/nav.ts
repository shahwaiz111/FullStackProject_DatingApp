import { Component, inject, signal } from '@angular/core';
import {FormsModule} from '@angular/forms'
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";


@Component({
  selector: 'app-nav',
  standalone:true,
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {

  protected accountService = inject(AccountService)
  // property to hold form inputs
  protected creds:any={}
  // protected loggedIn = signal(false)

  private router = inject(Router);


  login(){
    // console.log(this.creds);
    this.accountService.login(this.creds).subscribe({
      next: () => {
        // console.log(result);
        this.router.navigateByUrl('/members')
      },
      error: error => alert(error.message)
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
    this.creds = {};
  }
}
