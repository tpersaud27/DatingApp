import { AccountService } from './../../_services/account.service';
import { Pagination } from './../../_models/Pagination';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UserParams } from 'src/app/_models/UserParams';
import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/Member';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // This is an observable that we will subscribe to to return the members
  // members$: Observable<Member[]> | undefined;
  members: any;
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  user: User | undefined;
  genderList = [
    { value: 'male', display: 'Males' },
    {
      value: 'female',
      display: 'Females',
    },
  ];

  constructor(
    private membersService: MembersService,
    private accountService: AccountService
  ) {
    // Here we will get the current user and initialize our userParams for pagination
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      },
    });
  }

  ngOnInit(): void {
    // this.members$ = this.membersService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (!this.userParams) {
      return;
    }

    this.membersService.getMembers(this.userParams).subscribe({
      next: (response) => {
        if (response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      },
    });
  }

  resetFilters() {
    if (this.user) {
      // This will take the user back to page 1 with default ages and default gender
      this.userParams = new UserParams(this.user);
      this.loadMembers();
    }
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }
}
