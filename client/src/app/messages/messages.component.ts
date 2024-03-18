import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] | undefined;
  container: string = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;
  loading = false;
  constructor(private messageService: MessageService) { }
  ngOnInit(): void {
    this.getMessages();
  }
  getMessages() {
    this.loading = true;
    return this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(
      {
        next: (response) => {
          this.messages = response.result;
          this.pagination = response.pagination;
          this.loading = false;
        }
      }
    )
  }
  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: _ => {
        this.messages?.splice(this.messages.findIndex(u => u.id == id), 1);
      }
    })
  }
  pageChanged(event: any) {
    if (this.pageNumber != event.page) {
      this.pageNumber = event.page;
      this.getMessages();
    }
  }
}
