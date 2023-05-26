import { ConfirmationService } from './../_services/confirmation.service';
import { MemberEditComponent } from './../members/member-edit/member-edit.component';
import { Injectable } from '@angular/core';
import { CanDeactivate, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PreventUnsavedChangesGuard
  implements CanDeactivate<MemberEditComponent>
{
  constructor(private confirmService: ConfirmationService) {}

  canDeactivate(component: MemberEditComponent): Observable<boolean> {
    // We have access to the component properties
    // We can check if the form is dirty (the user has made modifications)
    if (component.editForm.dirty) {
      // confirm will return true or false depending on the use will select
      // return confirm(
      //   'Are you sure you want to continue? Any unsaved changes will be lost'
      // );

      return this.confirmService.confirm();
    }
    return of(true);
  }
}
