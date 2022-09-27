import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  // Will store the values inside here and anytime a subscribes to the observable, it will emit the last value inside of it
  // Or any many values as want from inside of it
  // In this case, we store just one user
  private currentUserSource = new ReplaySubject<User>(1);
  // The convention for setting observables is using the $ sign
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  // Send a model to login in
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        // If the user exist we will populate the user object into the browser local storage
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          // We are using this to persist the login
          this.currentUserSource.next(user);
        }
      })
    );
  }

  // Send a new user to the api to register them
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        // If the user exists
        if (user) {
          // Key is user, value is the json of the user object
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
