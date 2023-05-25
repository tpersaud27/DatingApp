import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;

  // At the time of the service is constructe we will not have the connection so this is why we use the optional chaining
  private hubConnection?: HubConnection;

  constructor(private toastr: ToastrService) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      // Client will retry connection to connect to the api
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => {
      console.log(error);
    });

    // This must match the name from the api
    this.hubConnection.on('UserIsOnline', (username) => {
      this.toastr.info(username + 'has connected!');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.toastr.warning(username + 'has disconnected!');
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
