import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  if (accountService.roles().includes('Admin') || accountService.roles().includes('Moderator')) {
    return true;
  }
  
  toastr.error('You do not have permission to access this page.');
  return false;
};
