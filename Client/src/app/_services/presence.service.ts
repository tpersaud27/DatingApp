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

  private hubConnection?: HubConnection;

  constructor(private toastr: ToastrService) { }
  

  createHubConnection(user: User) { 
    this.hubConnection = new HubConnectionBuilder(){
      .withUrl(this.hubUrl + "presence", {
        accesstokenFac
      })
    }
  }
}
