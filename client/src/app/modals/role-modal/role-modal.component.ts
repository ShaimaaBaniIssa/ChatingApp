import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-role-modal',
  templateUrl: './role-modal.component.html',
  styleUrls: ['./role-modal.component.css']
})
export class RoleModalComponent {
  userName: string = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) { }
  updateChecked(chekedValue: string) {
    const index = this.selectedRoles.indexOf(chekedValue);
    index !== -1 ?
      // at already exist , delete it
      this.selectedRoles.splice(index, 1) :
      this.selectedRoles.push(chekedValue);
  }

}
