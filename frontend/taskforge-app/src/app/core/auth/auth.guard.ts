import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

function isTokenValid(t: string | null): boolean {
  if (!t) return false;
  try {
    const payload = JSON.parse(atob(t.split('.')[1])); // decode JWT payload
    if (payload?.exp === undefined) return true;       // no exp -> treat as valid for dev
    const now = Math.floor(Date.now() / 1000);
    return now < payload.exp;
  } catch {
    return false; // bad token format
  }
}

export const authGuard: CanActivateFn = (route, state) => {
   const token = localStorage.getItem('access_token');
 const ok = isTokenValid(token);
  if (ok) return true;

  const router = inject(Router);
  return router.parseUrl('/auth');
};
