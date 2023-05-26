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

  messageContent: string = '';

  constructor(public messageService: MessageService) {}

  ngOnInit(): void {}

  sendMessage() {
    if (!this.username) {
      return;
    }

    this.messageService
      .sendMessage(this.username, this.messageContent)
      .then(() => {
        // After the message is set we just reset the form
        this.messageForm.reset();
      });
  }
}
