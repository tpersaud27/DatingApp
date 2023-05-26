import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Message } from './../_models/Message';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;

  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private httpClient: HttpClient) {}

  createHubConnection(user: User, otherUserName: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUserName, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('ReceivedMessageThread', (messages) => {
      this.messageThreadSource.next(messages);
    });

    // This will esentially create a new array adding the new message to the array
    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages) => {
          this.messageThreadSource.next([...messages, message]);
        },
      });
    });
  }

  stopHubConnection() {
    // Checking if we have a hub connection before we stop it
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResults<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.httpClient
    );
  }

  getMessageThread(username: string) {
    return this.httpClient.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }

  /**
   * Note: This endpoint will return a MessageDto for us
   * @param username This is the username of the recipient
   * @param content This is the content of the messaging the current user will send to the recipient
   */
  async sendMessage(username: string, content: string) {
    // return this.httpClient.post<Message>(this.baseUrl + 'messages', {
    //   recipientUsername: username,
    //   content: content,
    // });
    // Using the hub instead
    return this.hubConnection
      ?.invoke('SendMessage', {
        recipientUsername: username,
        content,
      })
      .catch((error) => console.log(error));
  }

  deleteMessage(id: number) {
    return this.httpClient.delete(this.baseUrl + 'messages/' + id);
  }
}
