import { Photo } from 'src/app/_models/Photo';
import { AccountService } from './../_services/account.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';

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
    private toastr: ToastrService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    // When the component is created we want to initialize the form
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: [
        '',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      // We need to ensure that the password and confirm password match
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    // This will allow us to ensure that if the password is changed even after the confirmPassword matches, any update to it will also check the validity again
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      },
    });
  }

  //This will be used to validate if the confirmPassword matches the password
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      // We are comparing one password control with another password control
      // In general we want to check if the control value of one matches that of the other
      // notMatching will be the name of the error
      // If the passwords match we return null otherwise we return a property notMatching with the value of true
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : { notMatching: true };
    };
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
