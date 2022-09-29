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
  canActivate(): Observable<boolean> {
    // Since we are in a auth guard it will handle the subscription by itself
    return this.accountService.currentUser$.pipe(
      map((user) => {
        // If the user exists return true
        if (user) return true;
        this.toastr.error('You shall not pass!');
      })
    );
  }
}
