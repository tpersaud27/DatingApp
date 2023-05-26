import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  // At the time of the service is constructe we will not have the connection so this is why we use the optional chaining
  private hubConnection?: HubConnection;
  // This behaviorSubject will emit to subscribes the current values stored
  // So in this case it will emit the online users
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  // This observable will give us something to subscribe to from our components
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) {}

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
      // this.toastr.info(username + ' has connected!');
      // Creating a new array of online users
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (usernames) => {
          // This will add the latest username to the list
          this.onlineUsersSource.next([...usernames, username]);
        },
      });
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      // this.toastr.warning(username + ' has disconnected!');

      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (usernames) => {
          // This will remove the latest username to the list
          // Filter method creates a new array not including the username removed
          this.onlineUsersSource.next(usernames.filter((x) => x !== username));
        },
      });
    });

    this.hubConnection.on('GetOnlineUsers', (usernames) => {
      // this will update the the onlineUsersSource with the usernames and notifies all observables that are subscribed
      // Note: https://dev.to/dipteekhd/angular-behaviorsubject-p1#:~:text=When%20a%20user%20performs%20any,who%20subscribed%20to%20source%20observable.
      this.onlineUsersSource.next(usernames);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr
        .info(knownAs + ' has sent you a new message! Click me to see it!')
        .onTap.pipe(take(1))
        .subscribe({
          next: () =>
            this.router.navigateByUrl('/members/' + username + '?tab=Messages'),
        });
    });
  }

  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
