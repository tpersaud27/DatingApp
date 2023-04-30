import { JsonPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  // Will store the values inside here and anytime a subscribes to the observable, it will emit the last value inside of it, with the intial value of null
  // In this case, we store just one user
  private currentUserSource = new BehaviorSubject<User | null>(null);
  // The convention for setting observables is using the $ sign
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  // Send a model to login in
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      // In here we will do something with the observable as it comes back from the api. This will be done before the subscription to this method
      map((response: User) => {
        const user: User = response;
        // If the user exist we will populate the user object into the browser local storage
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  // Send a new user to the api to register them
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        // If the user exists
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    user.roles = [];
    // Claim name is role
    const roles = this.getDecodedToken(user.token).role;
    // Check if there is multiple roles in the claim
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);
    // Key is user, value is the json of the user object
    // Once the user registers we log them in
    localStorage.setItem('user', JSON.stringify(user));
    // We are using this to persist the login
    this.currentUserSource.next(user);
  }

  /**
   * This will remove the user object from browser local storage when logging out
   */
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  getDecodedToken(token: string) {
    // This will give us the claim information from the token
    return JSON.parse(atob(token.split('.')[1]));
  }
}
