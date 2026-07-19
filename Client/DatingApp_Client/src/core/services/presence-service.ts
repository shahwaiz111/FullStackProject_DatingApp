import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../../types/user';
import * as signalR from '@microsoft/signalr'
import { Message } from '../../types/message';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  private hubUrl = environment.hubUrl;
  private toastr = inject(ToastrService);
  public hubConnection? : signalR.HubConnection
  private router = inject(Router);
  onlineUsers = signal<string[]>([]);

  createHubConnection(user:User){
    this.hubConnection = new signalR.HubConnectionBuilder().withUrl(this.hubUrl + 'presence',{
      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(error=> console.log(error));
    this.hubConnection.on('UserOnline', userId=>{
      // alert(email + ' ' + 'has connected');
      this.onlineUsers.update(users=> [...users, userId])
    });

    this.hubConnection.on('UserOffline', userId=>{
      this.onlineUsers.update(users=> users.filter(x=> x!== userId))
    });

    this.hubConnection.on('GetOnlineUsers', userIds=>{
      this.onlineUsers.set(userIds);
    })

    this.hubConnection.on('NewMessageReceived', (message:Message) =>{
       const toast = this.toastr.info(`${message.senderDisplayName} sent you a message`, 'New Message');
       toast.onTap.subscribe(()=>{
        this.router.navigate(['/member', message.senderId, 'messages']);
       });
      
      } 
      
    )
  }

  public stopHubConnection(){
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected){
      this.hubConnection.stop().catch(error=> console.log(error))
    }
  }
}
