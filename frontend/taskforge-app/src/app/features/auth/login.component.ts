import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from '../../core/api/auth.service';

@Component({
 standalone: true,
  selector: 'app-login',
  imports: [FormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email=''; password=''; error=''; busy=false;
  name=''; regEmail=''; regPass='';

  constructor(private auth: AuthService, private router: Router) {}

  login(){
    this.busy=true; this.error='';
    this.auth.login(this.email, this.password).subscribe({
      next: r => { localStorage.setItem('access_token', r.accessToken); this.router.navigateByUrl('/projects'); },
      error: _ => { this.error='Invalid credentials'; this.busy=false; }
    });
  }
  register(){
    this.auth.register(this.regEmail, this.regPass, this.name).subscribe({
      next: _ => { this.email=this.regEmail; this.password=this.regPass; this.login(); },
      error: e => this.error = e?.error ?? 'Registration failed'
    });
  }
}
