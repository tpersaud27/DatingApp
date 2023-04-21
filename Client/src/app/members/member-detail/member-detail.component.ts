import { MembersService } from 'src/app/_services/members.service';
import { Member } from 'src/app/_models/Member';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/Message';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  messages: Message[] = [];

  member: Member | undefined;
  galleryImages: NgxGalleryImage[] = [];
  galleryOptions: NgxGalleryOptions[] = [];

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent: 100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];
  }

  getImages() {
    // If there is no member exit method (this will mean galleryImages array is empty, no images)
    if (!this.member) return [];

    const imageUrls = [];

    // Loop over the images that the member has and add to array
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
      });
    }

    return imageUrls;
  }

  loadMember() {
    // This will assign userName to the route which has the paramter username
    const userName = this.route.snapshot.paramMap.get('username');

    if (!userName) return;

    this.memberService.getMemberByUserName(userName).subscribe({
      next: (member) => {
        this.member = member;
        // We want to ensure we have a member before we load the photos, thats why we are loading them here
        this.galleryImages = this.getImages();
      },
    });
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: (messages) => {
          this.messages = messages;
        },
      });
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;

    // If the active tab is the messages tab we will then load the messages
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    }
  }
}
