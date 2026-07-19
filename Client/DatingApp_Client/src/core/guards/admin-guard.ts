import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  if (accountService.currentUser()?.roles.includes('Admin') || accountService.currentUser()?.roles.includes('Moderator')){
    return true;
  } else{
    
    // alert("Cannot enter this area")
    toastr.error("you cannot enter this area")
    return false;
  }
};
