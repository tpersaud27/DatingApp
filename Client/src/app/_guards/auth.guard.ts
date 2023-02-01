import { map } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  // We are returning a observable that is a booleaan
  // We are checking if a user is logged in so they can access the different components
  canActivate(): Observable<boolean> {
    // Since we are in a auth guard it will handle the subscription by itself
    return this.accountService.currentUser$.pipe(
      map((user) => {
        // If the user exists return true
        // Since we return true here, the authGuard will return true and allow the user to proceed
        if (user) {
          return true;
        } else {
          this.toastr.error('You shall not pass!');
        }
      })
    );
  }
}
