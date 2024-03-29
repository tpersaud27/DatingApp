import { ToastrService } from 'ngx-toastr';
import { MembersService } from './../../_services/members.service';
import { User } from './../../_models/User';
import { AccountService } from './../../_services/account.service';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Member } from 'src/app/_models/Member';
import { take } from 'rxjs/operators';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  // This will be the information for the member model
  member: Member | undefined;
  // This will be what is return from the login endpoint. The model of the user (username and token)
  // Our user is null initially until we go to our account service and get the user
  user: User | null = null;

  // This will look for a template form with the name editForm
  @ViewChild('editForm') editForm: NgForm | undefined;
  // This is the browser event that notifies the user when they are leaving the page when they are
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ) {
    // As soon as we get the user our request is completed and we do not need to unsubscribe
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => (this.user = user),
    });
  }

  ngOnInit(): void {
    this.loadMember();
  }

  /**
   * This method retrieves the member details and stores it in the class property member
   * @returns Member details
   */
  loadMember() {
    // Exit the method if the user does not exist
    if (!this.user) return;

    this.memberService.getMemberByUserName(this.user.username).subscribe({
      next: (member) => (this.member = member),
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      // Since we get nothing back from this request as the response is 204. We just notify the user and update the form upon submission.
      next: (_) => {
        this.toastr.success('Profile Updated Successfully');
        // Update the user information when they submit the form
        this.editForm.reset(this.member);
      },
    });
  }
}
