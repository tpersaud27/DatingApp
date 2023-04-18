import { Component, OnInit } from '@angular/core';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/Member';
import { Pagination } from '../_models/Pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  members: any;
  predicate: string = 'liked';
  pageNumber: number = 1;
  pageSize: number = 5;
  pagination: Pagination | undefined;

  constructor(private memberService: MembersService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService
      .getLikes(this.predicate, this.pageNumber, this.pageSize)
      .subscribe({
        next: (response) => {
          this.members = response.result;
          this.pagination = response.pagination;
        },
      });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
