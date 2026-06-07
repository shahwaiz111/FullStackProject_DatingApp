import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Router, RouterOutlet } from '@angular/router';
import { NgClass } from '@angular/common';
import { LikesService } from '../core/services/likes-service';



@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, NgClass],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private accountService = inject(AccountService);
  protected router = inject(Router);
  // Angular DI
  private http = inject(HttpClient)
  private likeService = inject(LikesService);
  protected readonly title = signal('Dating App');
  // protected members:any;

  ngOnInit(): void {
    this.setCurrentUser();
    // this.http.get('https://localhost:5001/api/members/getallmembers').subscribe({
    //   next: response=> this.members = response,
    //   error: error=> console.log(error),
    //   complete: () => console.log('completed the Http Request..')
      
    // })
  }  

  setCurrentUser(){
    const userString = localStorage.getItem('user');
    
    if (!userString){
      return
    }
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
    this.likeService.getLikeIds();
  }


  }
