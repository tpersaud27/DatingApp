import { PaginatedResult } from './../_models/Pagination';
import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/Message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages?: any;
  pagination: Pagination | undefined;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;

  isLoading: boolean = false;

  constructor(private messageService: MessageService) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.isLoading = true;

    this.messageService
      .getMessages(this.pageNumber, this.pageSize, this.container)
      .subscribe({
        next: (response) => {
          this.messages = response.result;
          this.pagination = response.pagination;
          this.isLoading = false;
        },
      });
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => {
        // This will remove the element from our messages array
        this.messages?.splice(
          this.messages.findIndex((m) => m.id === id),
          1
        );
      },
    });
  }

  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
