import { MembersService } from 'src/app/_services/members.service';
import { Member } from 'src/app/_models/member';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    // This will assign userName to the route which has the paramter username
    const userName = this.route.snapshot.paramMap.get('username');

    if (!userName) return;

    this.memberService.getMemberByUserName(userName).subscribe({
      next: (member) => (this.member = member),
    });
  }
}
