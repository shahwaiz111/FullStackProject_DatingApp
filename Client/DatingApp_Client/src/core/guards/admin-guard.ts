import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);

  if (accountService.currentUser()?.roles.includes('Admin') || accountService.currentUser()?.roles.includes('Moderator')){
    return true;
  } else{
    alert("Cannot enter this area")
    return false;
  }
};
