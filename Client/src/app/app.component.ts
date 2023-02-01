import { AccountService } from './_services/account.service';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/User';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'Dating App Project';

  users: any;

  // We use the constructor for dependency injection
  // This should be used for dependency injection and initializig class members
  constructor(private accountService: AccountService) {}

  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    // We use parse to get an javascript object from JSON string
    const user: User = JSON.parse(localStorage.getItem('user'));
    // We can set the current user
    this.accountService.setCurrentUser(user);
  }
}
