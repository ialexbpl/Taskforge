import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProjectsService {
  private base = environment.apiBaseUrl + '/projects';
  constructor(private http: HttpClient) {}
  list() { return this.http.get<any[]>(this.base); }
  create(name: string, key: string) { return this.http.post<any>(this.base, { name, key }); }
  get(id: string) { return this.http.get<any>(`${this.base}/${id}`); }
}
