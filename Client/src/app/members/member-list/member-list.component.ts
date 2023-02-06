import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // This property will be a array of members
  members: Member[];

  constructor(private membersService: MembersService) {}

  ngOnInit(): void {
    // Initially we want members in our application
    this.loadMembers();
  }

  // Load Members
  // We know that getMembers returns a array of members so we just set that equal to our class property
  loadMembers() {
    this.membersService.getMembers().subscribe({
      next: (members) => (this.members = members),
    });
  }
}
