import { Component, inject, OnDestroy, OnInit, signal, ViewChild, viewChild } from '@angular/core';
// import { ActivatedRoute } from '@angular/router';
import { EditableMember, Member } from '../../../types/member';
import { DatePipe } from '@angular/common';
import { MemberService } from '../../../core/services/member-service';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-profile', 
  imports: [DatePipe, FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {
  
  
  @ViewChild('editForm') editForm?:NgForm;
  private accountService = inject(AccountService);
  protected memberService = inject(MemberService);
  // private route = inject(ActivatedRoute);
  // protected member = signal<Member | undefined>(undefined);
  protected editableMember: EditableMember = {
    displayName: '',
    description: '',
    city: '',
    country: ''
  }

  

  ngOnInit(): void {
//     this.route.parent?.data.subscribe(data => {
//       this.member.set(data['member']);
// })
this.editableMember = {
        displayName: this.memberService.member()?.displayName || '',
        description: this.memberService.member()?.description || '',
        city: this.memberService.member()?.city || '',
        country: this.memberService.member()?.country || '',
      }
    
  }
    updateProfile(){
      if (!this.memberService.member()) return;
      const updatedMember = {...this.memberService.member(), ...this.editableMember}
      this.memberService.updateMember(this.editableMember).subscribe({
        next: () => {
          const currentUser = this.accountService.currentUser();
          if (currentUser && updatedMember.displayName != currentUser?.displayName){
            currentUser.displayName = updatedMember.displayName;
            this.accountService.setCurrentUser(currentUser);
          }
          this.memberService.editMode.set(false);
          this.memberService.member.set(updatedMember as Member);
          this.editForm?.reset(updatedMember);
        }
      })
      // console.log(updatedMember);
      
  }
  ngOnDestroy(): void { // when we destroy a component, the code thats in the NgonDestroy is executed..
    if (this.memberService.editMode()){
      this.memberService.editMode.set(false);
    }
  }

}
