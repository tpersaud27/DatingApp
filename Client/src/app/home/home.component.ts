import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode: boolean = false;

  constructor() {}

  ngOnInit(): void {}

  // This just sets the registerMode boolean to the opposite
  // Toggling the registerMode
  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  // This is just to display passing data to child components (register component)
  // getUsers() {
  //   // This is the observer pattern
  //   this.http.get('https://localhost:5001/api/users').subscribe({
  //     next: (response) => (this.users = response),
  //     error: (error) => console.log(error),
  //   });
  // }

  // This will take a event in, which will be false and set the to the registerMode to disable the form
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
