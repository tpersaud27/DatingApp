import { Observable, of } from 'rxjs';
import { JwtInterceptor } from './../_interceptors/jwt.interceptor';
import { Member } from './../_models/Member';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;

  // Since Services are availble for the lifetime of the application, unlike components. We will be using them to store the members that are retrived from the DB
  // This allows us to not make extra calls when we already have the required member data
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  /**
   * Note: We use the [] brackets to signify this is a array of users
   * Note: Our interceptor will get the outgoing request and add the authentication token to the request
   * @returns
   */
  getMembers() {
    // If there are members already retrived
    if (this.members.length > 0) {
      // Since we need to be returning a observable we will be using 'of' from rxjs
      // This converts the property into an observable
      return of(this.members);
    }
    // If there are no members retrieved we would just return the members
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map((members) => {
        this.members = members;
        return members;
      })
    );
  }

  getMemberByUserName(username: string) {
    // We can check if the user is found in the members retrieved
    const member = this.members.find((x) => x.userName === username);
    if (member) {
      return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/username/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        // This will update the current member with the new information
        this.members[index] = { ...this.members[index], ...member };
      })
    );
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
