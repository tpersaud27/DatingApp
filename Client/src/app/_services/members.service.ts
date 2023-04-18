import { User } from 'src/app/_models/User';
import { PaginatedResult } from './../_models/Pagination';
import { of } from 'rxjs';
import { Member } from './../_models/Member';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { map, take } from 'rxjs/operators';
import { UserParams } from '../_models/UserParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;

  // Since Services are availble for the lifetime of the application, unlike components. We will be using them to store the members that are retrived from the DB
  // This allows us to not make extra calls when we already have the required member data
  members: Member[] = [];

  memberCache = new Map();

  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    // Here we will get the current user and initialize our userParams for pagination
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      },
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  /**
   * Note: We use the [] brackets to signify this is a array of users
   * Note: Our interceptor will get the outgoing request and add the authentication token to the request
   * Note: the page number and itemsPerPage are options because the server has default values
   * @returns
   */
  getMembers(userParams: UserParams) {
    // We will check if this query has been made before
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    // Add the min age
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResults<Member[]>(
      this.baseUrl + 'users',
      params
    ).pipe(
      map((response) => {
        // We will set the memberCache
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  getMemberByUserName(username: string) {
    // This will flatten the arrays into one array holding the result
    // This becomes a array of members
    const member = [...this.memberCache.values()]
      .reduce((arr, element) => arr.concat(element.result), [])
      .find((member: Member) => member.userName === username);

    if (member) return of(member);

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

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string) {
    return this.http.get<Member[]>(
      this.baseUrl + 'likes?predicate=' + predicate
    );
  }

  // We made this method more generic using T
  private getPaginatedResults<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T[]> = new PaginatedResult<T[]>();

    return (
      this.http
        // Doing this will allow us to get the response instead of just the http body
        .get<T[]>(url, { observe: 'response', params })
        .pipe(
          map((response) => {
            if (response.body) {
              // Remeber that result is the list of items we will get back, this will be the body of the request
              paginatedResult.result = response.body;
            }
            const pagination = response.headers.get('Pagination');
            if (pagination) {
              // We use JSON parse to convert the json to a object
              paginatedResult.pagination = JSON.parse(pagination);
            }
            return paginatedResult;
          })
        )
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    // HttpParams allows us to set query strings
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return params;
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
