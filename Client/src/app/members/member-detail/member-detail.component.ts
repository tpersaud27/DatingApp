import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from 'src/app/_models/Member';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/Message';
import { PresenceService } from 'src/app/_services/presence.service';
import { User } from 'src/app/_models/User';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  // Note: We do not have access to viewchild properties until after the component is constructed
  // Instead of waiting for the view to load dynamically it will be statically loaded
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  messages: Message[] = [];
  // This is the member the user is looking at the details of
  member: Member = {} as Member;
  galleryImages: NgxGalleryImage[] = [];
  galleryOptions: NgxGalleryOptions[] = [];

  user?: User;

  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService: PresenceService,
    private router: Router
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
    // This will not allow a route to be reused, forcing a reload
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    // this.loadMember();
    // Instead of using loadMember we will get getting it from our route instead
    // This is done by using route resolvers
    this.route.data.subscribe({
      next: (data) => {
        this.member = data['member'];
      },
    });

    // Access the route to get the queryParams
    // This will select the specified tab in the query params
    this.route.queryParams.subscribe({
      next: (params) => {
        params['tab'] && this.selectTab(params['tab']);
      },
    });

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

    this.galleryImages = this.getImages();
  }

  // When the component is destroyed the connection is stopped
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
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

  // When this is passed in a heading, it will make that tab activated
  // The use case here is when a user is clicking on the messages button it will activate the messages tab
  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find((x) => x.heading === heading)!.active = true;
    }
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
    if (this.activeTab.heading === 'Messages' && this.user) {
      // this.loadMessages();
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  // This is no longer needed because we will get the member from the route resolver
  // loadMember() {
  //   // This will assign userName to the route which has the paramter username
  //   const userName = this.route.snapshot.paramMap.get('username');

  //   if (!userName) return;

  //   this.memberService.getMemberByUserName(userName).subscribe({
  //     next: (member) => {
  //       this.member = member;
  //       // We want to ensure we have a member before we load the photos, thats why we are loading them here
  //       this.galleryImages = this.getImages();
  //     },
  //   });
  // }
}
