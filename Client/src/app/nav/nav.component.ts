import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { BsDropdownConfig } from 'ngx-bootstrap/dropdown';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  // This what the user will submit
  model: any = {};

  // We made this accountService public to avoid having duplicate code
  // We can now access the accountService in our template instead
  // Note: Private services are only avaible in the component
  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    this.accountService.login(this.model).subscribe((response) => {
      this.router.navigateByUrl('/members');
      this.toastr.success('Logged In!');
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
