import { AccountService } from 'src/app/_services/account.service';
import {
  Directive,
  Input,
  TemplateRef,
  ViewContainerRef,
  OnInit,
} from '@angular/core';
import { User } from '../_models/User';
import { take } from 'rxjs/operators';

@Directive({
  selector: '[appHasRole]', //*appHasRole = '["Admin" , "Moderator"]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService
  ) {
    // Getting the current user
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }
  ngOnInit(): void {
    // If the user role has at least some role matching the roles specified in appHasRole (Admin and Moderator) then we display the contents
    if (this.user.roles.some((r) => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      // The admin link page will be removed from the DOM
      this.viewContainerRef.clear();
    }
  }
}
