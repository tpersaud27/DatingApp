import { Router } from '@angular/router';
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
  // This will be out reactive form used for registering the user
  registerForm: FormGroup = new FormGroup({});
  // This will be the maxDate for the user age (user must be at least 18 to register)
  maxDate: Date = new Date();
  // This will hold any errors returned from the server
  // In theory we shouldnt be getting any errors
  validationErrors: string[] | undefined;

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    // When the component is created we want to initialize the form
    this.initializeForm();
    // This will set the maximum year to the current year - 18. Limiting the user age to sign up
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
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
  // Note: Once the user is registered we will consider them logged in
  register() {
    // We get the date of birth from the datepicker form value
    const dob = this.getDateOnly(
      this.registerForm.controls['dateOfBirth'].value
    );
    // This will spread the registerForm values into individual properties
    // Here we can then override the value of the dateOfBirth with the fixed version
    const values = { ...this.registerForm.value, dateOfBirth: dob };

    this.accountService.register(values).subscribe({
      next: (response) => {
        // Instead of cancelling the form after registering, we will redirect the user to the members page
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        console.log(error);

        // Instead of using a toastr notification
        // We will store a array of errors that will be returned from the register service
        this.validationErrors = error;

        // this.toastr.error(error);
      },
    });
  }

  // We want to emit a value when we click on the cancel button
  cancel() {
    // We emit false because we want to send this to the home component
    this.cancelRegister.emit(false);
  }

  // This method will remove any unnecessary timings appending to the dateOfBirth from the date picker
  private getDateOnly(dateOfBirth: string) {
    if (!dateOfBirth) {
      return;
    }

    // We will first convert the dateOfBirth to a date type
    // Note at this point the dateOfBirth still contains the time stamp
    let dob = new Date(dateOfBirth);
    // This will allow us to get just the dateOfBirth with the first 10 characters removing the time stamp
    return new Date(dob.setMinutes(dob.getMinutes() - dob.getTimezoneOffset()))
      .toISOString()
      .slice(0, 10);
  }
}
