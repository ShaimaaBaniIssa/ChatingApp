import { Component, Input } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  @Input() member: Member | undefined;
  constructor(private memberService: MembersService, private toastr: ToastrService) { }
  addLike(userName: string) {
    this.memberService.addLike(userName).subscribe(
      {
        next: _ => {
          this.toastr.success('You have liked ' + userName);
        }
      }
    )
  }
}
