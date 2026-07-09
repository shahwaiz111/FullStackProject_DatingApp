import { inject, Injectable } from '@angular/core';
import { AccountService } from './account-service';
import { LikesService } from './likes-service';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private accountService = inject(AccountService);
  private likeService = inject(LikesService);
  

  init(){
    return this.accountService.refreshToken().pipe(
      tap(user => {
        if (user){
          // this.accountService.setCurrentUser(user);
          // this.likeService.getLikeIds();
          this.accountService.setCurrentUser(user);
          this.accountService.startTokenRefreshInterval();
        }
      })
    )
  }
}
