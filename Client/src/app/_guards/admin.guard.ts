import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/_services/account.service';
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((user) => {
        if (!user) {
          return false;
        }
        // If the user is either Admin or moderator they can proceed
        if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
          return true;
        }
        // Otherwise they are not allowed to proceed
        else {
          this.toastr.error('You cannot enter this area');
          return false;
        }
      })
    );
  }
}
