import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { TimeagoModule } from "ngx-timeago";
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';

@Component({
  selector: 'app-member-detail',
  standalone: true, // no longer part of ng module
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  messages: Message[] = [];

  constructor(private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessageService) { }
  ngOnInit(): void {

    //route resolver get the data before a component is constructed
    // The router waits for the data to be resolved before the route is finally activated
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'] // same property name in route resolver
      }
    })
    // happened before View initalization
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])

      }
    })
    this.getImages();


  }
  onTabActivatd(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab?.heading === 'Messages') {
      this.getMessages()
    }
  }
  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(u => u.heading === heading)!.active = true;
    }
  }
  getMessages() {
    if (this.member?.userName) {
      this.messageService.getMessageThread(this.member?.userName).subscribe({
        next: (response) => {
          this.messages = response;
        }
      })
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({
        src: photo.url, thumb: photo.url
      }))
    }
  }
}
