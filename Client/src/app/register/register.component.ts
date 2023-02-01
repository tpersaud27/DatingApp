import { AccountService } from './../_services/account.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { ToastrService } from 'ngx-toastr';

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

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  // When the form is submitted, the data from the form will be sent to this method
  register() {
    this.accountService.register(this.model).subscribe({
      next: (response) => {
        // After registering we cancel the form
        this.cancel();
      },
      error: (error) => {
        console.log(error);
        this.toastr.error(error.error);
      },
    });
  }

  // We want to emit a value when we click on the cancel button
  cancel() {
    // We emit false because we want to send this to the home component
    this.cancelRegister.emit(false);
  }
}
