import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root'
})
export class IssuesService {
  private base = environment.apiBaseUrl + '/issues';
  constructor(private http: HttpClient) {}
  byProject(projectId: string) { return this.http.get<any[]>(`${this.base}/by-project/${projectId}`); }
  create(dto: any) { return this.http.post<any>(this.base, dto); }
  setStatus(id: string, status: string) { return this.http.patch<any>(`${this.base}/${id}/status/${status}`, {}); }
    get(id: string)              { return this.http.get<any>(`${this.base}/${id}`); }
  update(id: string, dto: any) { return this.http.put<any>(`${this.base}/${id}`, dto); }
  listComments(id: string)     { return this.http.get<any[]>(`${this.base}/${id}/comments`); }
  addComment(id: string, body: string) { return this.http.post<any>(`${this.base}/${id}/comments`, { body }); }
}
