import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
 selector: 'app-root',
  standalone: true,
  // added RouterLink & RouterLinkActive so links work in a standalone component
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar navbar-expand navbar-light bg-light px-3">
      <a class="navbar-brand" routerLink="/">TaskForge</a>

      <div class="navbar-nav">
        <a class="nav-link" routerLink="/projects" routerLinkActive="active" [routerLinkActiveOptions]="{ exact: true }">Projects</a>
        <a class="nav-link" routerLink="/auth" routerLinkActive="active">Auth</a>
      </div>

      <div class="ms-auto">
        <button class="btn btn-sm btn-outline-secondary" (click)="logout()">Logout</button>
      </div>
    </nav>

    <router-outlet></router-outlet>
  `
})
export class AppComponent {
   constructor(private router: Router) {}
  logout() {
    localStorage.removeItem('access_token');
    this.router.navigateByUrl('/auth');
}
}
