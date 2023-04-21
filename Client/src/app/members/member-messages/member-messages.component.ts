import { NgForm } from '@angular/forms';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from 'src/app/_models/Message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;

  // This is the username of the person the current user is looking at
  @Input()
  username?: string;
  @Input()
  messages: Message[] = [];

  messageContent: string = '';

  constructor(private messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    if (!this.username) {
      return;
    }

    this.messageService
      .sendMessage(this.username, this.messageContent)
      .subscribe({
        next: (message) => {
          // We will get a messageDto back, this can be added to our messages array
          // This is to update the front-end user of the new messages
          this.messages.push(message);

          // Now we clear the form
          this.messageForm?.reset();
        },
      });
  }
}
