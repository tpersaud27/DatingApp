import { JwtInterceptor } from './../_interceptors/jwt.interceptor';
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
   * Note: We use the [] brackets to signify this is a array of users
   * Note: Our interceptor will get the outgoing request and add the authentication token to the request
   * @returns
   */
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMemberByUserName(userName: string) {
    return this.http.get<Member>(this.baseUrl + 'users/username/' + userName);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member);
  }

  // Temp method to pass token in the http header
  // getHttpOptions() {
  //   // Getting the current user.
  //   const userString = localStorage.getItem('user');
  //   if (!userString) return;

  //   const user = JSON.parse(userString);
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: 'Bearer ' + user.token,
  //     }),
  //   };
  // }
}
