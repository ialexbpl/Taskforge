import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private base = environment.apiBaseUrl + '/auth';
  constructor(private http: HttpClient) {}
  login(email: string, password: string) {
    return this.http.post<{accessToken:string}>(`${this.base}/login`, { email, password });
  }
  register(email: string, password: string, displayName: string) {
    return this.http.post(`${this.base}/register`, { email, password, displayName });

  }}