import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { filter, Observable } from 'rxjs';
import { Member } from '../../../types/member';
import { AccountService } from '../../../core/services/account-service';
import { PresenceService } from '../../../core/services/presence-service';
import { LikesService } from '../../../core/services/likes-service';

@Component({
  selector: 'app-members-detailed',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './members-detailed.html',
  styleUrl: './members-detailed.css',
})
export class MembersDetailed implements OnInit {
  
  protected memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  protected presenceService = inject(PresenceService);
  protected likeService = inject(LikesService);
  private router = inject(Router);
  // protected member$?: Observable<Member>;
  // protected member = signal<Member | undefined>(undefined);
  protected title = signal<string | undefined>('Profile')
  private routeId = signal<string | null>(null);
  
 
  protected hasLiked = computed(() => this.likeService.likeIds().includes(this.routeId()!)); 
  
   // if this matches then we are working with current profile.. comparing current user id..
  protected isCurrentUser = computed(()=>{
      // return this.accountService.currentUser()?.id === this.route.snapshot.paramMap.get('id');
      return this.accountService.currentUser()?.id === this.routeId()
  });
  
  constructor(){
    this.route.paramMap.subscribe(params=>{
      this.routeId.set(params.get('id'));
    })
  }

  ngOnInit(): void {
    // this.member$ = this.loadMember();
    // this.route.data.subscribe({
    //   next: data => this.member.set(data['member'])
    // })

    this.title.set(this.route.firstChild?.snapshot.title)

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)).subscribe({
        next: () => {
          this.title.set(this.route.firstChild?.snapshot?.title)
        }
      })
    
  }

  // loadMember(){
  //   const id = this.route.snapshot.paramMap.get('id');
  //   if (!id) return;
  //   return this.memberService.getMember(id);
  // }

}
