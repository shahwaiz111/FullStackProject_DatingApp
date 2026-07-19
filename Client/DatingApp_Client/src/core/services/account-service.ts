import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';
import { PresenceService } from './presence-service';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root', // means it automatically going to be provided to our application as a whole..
})
export class AccountService {
    currentUser = signal<User | null>(null);  
    private http = inject(HttpClient)
    private likesService = inject(LikesService);
    private presenceService = inject(PresenceService);
    // baseUrl = 'https://localhost:5001/api/'
    private baseUrl = environment.apiUrl;

    register(creds:RegisterCreds){
      return this.http.post<User>(this.baseUrl + 'account/register', creds, {withCredentials: true}).pipe( // when the response come back do something extra with it..
        tap(user =>{
          if (user){
            this.setCurrentUser(user);
            this.startTokenRefreshInterval();
          }
        })
      )
    }

    login(creds:LoginCreds){
      return this.http.post<User>(this.baseUrl + 'account/login', creds, {withCredentials:true}).pipe( // when the response come back do something extra with it..
        tap(user =>{
          if (user){
            this.setCurrentUser(user);
            this.startTokenRefreshInterval(); 
          }
        })
      )
    }

    refreshToken(){
      return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials:true})
    }

    startTokenRefreshInterval(){
      setInterval(()=>{
        this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials:true}).subscribe({
          next: user=>{
            this.setCurrentUser(user)
          },
          error: ()=>{
            this.logout()
          }
        })
      }, 5 * 60 * 1000)
    }

    setCurrentUser(user:User){
      user.roles = this.getRolesFromToken(user);
      // localStorage.setItem('user', JSON.stringify(user))
      this.currentUser.set(user);
      this.likesService.getLikeIds();
      if (this.presenceService.hubConnection?.state !== HubConnectionState.Connected){
        this.presenceService.createHubConnection(user)
      }
    }

    logout(){
      // localStorage.removeItem('user');
      localStorage.removeItem('filters');
      this.likesService.clearLikeIds();  
      this.presenceService.stopHubConnection();  
      this.currentUser.set(null);
    }

    private getRolesFromToken(user:User):string[]{
      const payLoad = user.token.split('.')[1];
      const decoded = atob(payLoad);
      const jsonPayLoad = JSON.parse(decoded);
      return Array.isArray(jsonPayLoad.role) ? jsonPayLoad.role : [jsonPayLoad.role]
    }
}
