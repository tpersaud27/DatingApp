import { Member } from './../_models/Member';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Note we use the [] brackets to signify this is a array of users
   * @returns
   */
  getMembers() {
    return this.http.get<Member[]>(
      this.baseUrl + 'users',
      this.getHttpOptions()
    );
  }

  getMemberByUserName(userName: string) {
    return this.http.get<Member>(
      this.baseUrl + 'users/username/' + userName,
      this.getHttpOptions()
    );
  }

  // Temp method to pass token in the http header
  getHttpOptions() {
    // Getting the current user.
    const userString = localStorage.getItem('user');
    if (!userString) return;

    const user = JSON.parse(userString);
    return {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + user.token,
      }),
    };
  }
}
