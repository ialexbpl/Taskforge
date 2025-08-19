import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService, LoginResponse } from '../../core/api/auth.service';

@Component({
 standalone: true,
  selector: 'app-login',
  imports: [FormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  // login
  email = '';
  password = '';
  // register
  name = '';
  regEmail = '';
  regPass = '';

  error = '';
  busy = false;

  constructor(private auth: AuthService, private router: Router) {}

 login() {
    this.busy = true; this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: (r: LoginResponse) => {
        localStorage.setItem('access_token', r.accessToken);
        this.router.navigateByUrl('/projects');
      },
      error: () => { this.error = 'Invalid credentials'; this.busy = false; }
    });
  }
  register() {
    this.busy = true; this.error = '';
    this.auth.register(this.regEmail, this.regPass, this.name).subscribe({
      next: () => {
        // auto-login
        this.email = this.regEmail;
        this.password = this.regPass;
        this.login();
      },
      error: (e) => { this.error = e?.error ?? 'Registration failed'; this.busy = false; }
    });
  }
}
