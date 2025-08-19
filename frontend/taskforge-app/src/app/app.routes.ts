import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'auth', loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent) },
  { path: 'projects', loadComponent: () => import('./features/projects/list.component').then(m => m.ProjectListComponent) },
  { path: 'board/:projectId', loadComponent: () => import('./features/issues/board.component').then(m => m.BoardComponent) },
  { path: '', pathMatch: 'full', redirectTo: 'projects' }
];
