import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
   const token = localStorage.getItem('access_token');
  if (token) return true;
  inject(Router).navigateByUrl('/auth');
  return false;
};
