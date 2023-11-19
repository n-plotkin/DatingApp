import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  constructor(private toastr: ToastrService) { }
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      //hub url is configured on our server side
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(error => console.log(error));

      //the on is to listen for a specific event name on hubConnection
      this.hubConnection.on('UserIsOnline', username => {
        this.toastr.info(username + ' has connected');
      })

      this.hubConnection.on('userIsOffline', username => {
        this.toastr.warning(username + ' has disconnected')
      })

      this.hubConnection.on('GetOnlineUsers', usernames =>{
        this.onlineUsersSource.next(usernames);
      })
      

  }
  
  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }

}
