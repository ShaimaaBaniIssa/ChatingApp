import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent {
  @Input() username?: string;
  @Input() messages: Message[] = [];
  content = '';
  @ViewChild('messageForm') messageForm?: NgForm;
  constructor(private messageService: MessageService) { }

  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.content, this.username).subscribe({
      next: (message) => {
        this.messages.push(message);
        this.messageForm?.reset();
      }
    })
  }

}
