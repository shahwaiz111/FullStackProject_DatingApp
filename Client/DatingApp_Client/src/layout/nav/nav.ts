import { Component, inject, OnInit, signal } from '@angular/core';
import {FormsModule} from '@angular/forms'
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { themes } from '../theme';
import { BusyService } from '../../core/services/busy-service';
import { HasRole } from '../../shared/directives/has-role';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-nav',
  standalone:true,
  imports: [FormsModule, RouterLink, RouterLinkActive, HasRole],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit {
  

  protected accountService = inject(AccountService)
  protected busyService = inject(BusyService);
  // property to hold form inputs
  protected creds:any={}
  // protected loggedIn = signal(false)
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;
  protected loading = signal(false);
  protected toast = inject(ToastrService);

  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  } 

  handleSelectTheme(theme:string){
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }

  handleSelectUserItem(){
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }

  private router = inject(Router);


  login(){
    // console.log(this.creds);
    this.loading.set(true);
    this.accountService.login(this.creds).subscribe({
      next: () => {
        // console.log(result);
        this.router.navigateByUrl('/members')
        this.toast.success("Logged in successfully!")

      },
      error: error => alert(error.message),
      complete: ()=> this.loading.set(false)
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
    this.creds = {};
  }
}
