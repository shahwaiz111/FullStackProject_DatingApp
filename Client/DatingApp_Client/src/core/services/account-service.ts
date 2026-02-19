import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root', // means it automatically going to be provided to our application as a whole..
})
export class AccountService {
    currentUser = signal<User | null>(null);  
    private http = inject(HttpClient)

    baseUrl = 'https://localhost:5001/api/'

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
    }

    logout(){
      localStorage.removeItem('user');  
      this.currentUser.set(null);  
    }

    
}
