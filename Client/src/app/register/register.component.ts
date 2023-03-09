import { Photo } from 'src/app/_models/Photo';
import { AccountService } from './../_services/account.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  // We use the input decorator to signal we are getting some data from a parent component
  // @Input() usersFromHomeComponent: any;
  // We can use the output decorator to signal we are sending some data to the home components
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  // This will be out reactive form used for registering the user
  registerForm: FormGroup = new FormGroup({});

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    // When the component is created we want to initialize the form
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl(),
      password: new FormControl(),
      confirmPassword: new FormControl(),
    });
  }

  // When the form is submitted, the data from the form will be sent to this method
  register() {
    // This will output the form values
    console.log(this.registerForm?.value);

    // this.accountService.register(this.model).subscribe({
    //   next: (response) => {
    //     // After registering we cancel the form
    //     this.cancel();
    //   },
    //   error: (error) => {
    //     console.log(error);
    //     this.toastr.error(error);
    //   },
    // });
  }

  // We want to emit a value when we click on the cancel button
  cancel() {
    // We emit false because we want to send this to the home component
    this.cancelRegister.emit(false);
  }
}
