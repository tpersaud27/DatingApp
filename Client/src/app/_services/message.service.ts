import { Message } from './../_models/Message';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResults, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) {}

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
  sendMessage(username: string, content: string) {
    return this.httpClient.post<Message>(this.baseUrl + 'messages', {
      recipientUsername: username,
      content: content,
    });
  }

  deleteMessage(id: number) {
    return this.httpClient.delete(this.baseUrl + 'messages/' + id);
  }
}
