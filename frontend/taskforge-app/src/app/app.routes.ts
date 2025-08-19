import { Routes } from '@angular/router';
import {authGuard} from './core/auth/auth.guard';

export const routes: Routes = [
    { path: 'auth', loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent) },
 { path: 'projects', canActivate: [authGuard], loadComponent: () => import('./features/projects/list.component').then(m => m.ProjectListComponent) },
  { path: 'board/:projectId', canActivate: [authGuard], loadComponent: () => import('./features/issues/board.component').then(m => m.BoardComponent) },
   // default landing page
  { path: '', pathMatch: 'full', redirectTo: 'auth' }
];
