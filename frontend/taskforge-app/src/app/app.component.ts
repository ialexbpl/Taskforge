import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
 selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <nav class="navbar navbar-expand navbar-light bg-light px-3">
      <a class="navbar-brand" routerLink="/">TaskForge</a>
      <div class="navbar-nav">
        <a class="nav-link" routerLink="/projects">Projects</a>
        <a class="nav-link" routerLink="/auth">Auth</a>
      </div>
      <div class="ms-auto">
        <button class="btn btn-sm btn-outline-secondary" (click)="logout()">Logout</button>
      </div>
    </nav>
    <router-outlet></router-outlet>`
})
export class AppComponent {
 logout(){ localStorage.removeItem('access_token'); }
