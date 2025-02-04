import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [MemberCardComponent, PaginationModule, FormsModule, ButtonsModule],
  templateUrl: './members-list.component.html',
  styleUrl: './members-list.component.css'
})
export class MembersListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList = [{value: 'male', display: 'Males'}, {value:'female', display: 'Females'}]

  ngOnInit(): void {
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers();
  }

  resetFilter() {
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.memberService.userParams().pageNumber !== event.page) {
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }

}
