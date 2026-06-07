import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';

@Injectable({
  providedIn: 'root', // means it automatically going to be provided to our application as a whole..
})
export class AccountService {
    currentUser = signal<User | null>(null);  
    private http = inject(HttpClient)
    private likesService = inject(LikesService);
    // baseUrl = 'https://localhost:5001/api/'
    private baseUrl = environment.apiUrl;

    register(creds:RegisterCreds){
      return this.http.post<User>(this.baseUrl + 'account/register', creds).pipe( // when the response come back do something extra with it..
        tap(user =>{
          if (user){
            this.setCurrentUser(user);
          }
        })
      )
    }

    login(creds:LoginCreds){
      return this.http.post<User>(this.baseUrl + 'account/login', creds).pipe( // when the response come back do something extra with it..
        tap(user =>{
          if (user){
            this.setCurrentUser(user);
          }
        })
      )
    }

    setCurrentUser(user:User){
      localStorage.setItem('user', JSON.stringify(user))
      this.currentUser.set(user);
      this.likesService.getLikeIds();
    }

    logout(){
      localStorage.removeItem('user');
      localStorage.removeItem('filters');
      this.likesService.clearLikeIds();  
      this.currentUser.set(null);  
    }

    
}
