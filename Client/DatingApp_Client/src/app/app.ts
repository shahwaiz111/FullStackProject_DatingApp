import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Router, RouterOutlet } from '@angular/router';
import { NgClass } from '@angular/common';
import { LikesService } from '../core/services/likes-service';
import { ConfirmDialog } from "../shared/confirm-dialog/confirm-dialog";



@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, NgClass, ConfirmDialog],
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
