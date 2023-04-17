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
  genderList = [
    { value: 'male', display: 'Males' },
    {
      value: 'female',
      display: 'Females',
    },
  ];

  constructor(private membersService: MembersService) {
    this.userParams = this.membersService.getUserParams();
  }

  ngOnInit(): void {
    // this.members$ = this.membersService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams) {
      this.membersService.setUserParams(this.userParams);

      this.membersService.getMembers(this.userParams).subscribe({
        next: (response) => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        },
      });
    }
  }

  resetFilters() {
    // This will take the user back to page 1 with default ages and default gender
    this.userParams = this.membersService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.userParams && this.userParams.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      this.membersService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
