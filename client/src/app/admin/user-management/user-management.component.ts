import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RoleModalComponent } from 'src/app/modals/role-modal/role-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef<RoleModalComponent> = new BsModalRef<RoleModalComponent>();
  availableRoles = [
    'Admin', 'Moderator', 'Member'
  ];


  constructor(private adminService: AdminService,
    private modalService: BsModalService) { }
  ngOnInit(): void {
    this.getUsersRoles();
  }
  getUsersRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: (users) => {
        if (users)
          this.users = users;
      }
    })
  }
  openRoleModal(user: User) {

    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        userName: user.userName,
        selectedRoles: [...user.roles],
        availableRoles: this.availableRoles
      }
    }
    this.bsModalRef = this.modalService.show(RoleModalComponent, config);
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content!.selectedRoles;
        if (!this.arrayEqual(selectedRoles, user.roles)) {
          this.adminService.editRoles(user.userName, selectedRoles!.join(',')).subscribe({
            next: (roles) => {
              user.roles = roles;
            }
          })
        }
      }
    })

  }
  private arrayEqual(array1: any, array2: any[]) {
    return JSON.stringify(array1.sort()) === JSON.stringify(array2.sort());
  }
}
