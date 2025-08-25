import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { IssuesService } from '../../core/api/issues.service';
import { JsonPipe } from '@angular/common';
type Issue = { id:string, title:string, status:string };

@Component({
  standalone: true,
  selector: 'app-board',
  imports: [FormsModule, NgFor, NgIf, JsonPipe],
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent {
  projectId = '';
  issues: Issue[] = [];
  columns = ['Backlog','Selected','InProgress','Review','Done'];
  title=''; desc='';
  error = '';            // <- string only for UI
  errorObj: any = null;  // <- keep raw error for debugging if needed

  constructor(private route: ActivatedRoute, private issuesApi: IssuesService) {}

  ngOnInit(){
    this.projectId = this.route.snapshot.paramMap.get('projectId') ?? '';
    if (!this.projectId) {
      this.error = 'Missing projectId in route';
      return;
    }
    this.reload();
  }

  private formatErr(e: any): string {
    this.errorObj = e; // keep raw for JSON view
    if (!e) return 'Unknown error';
    // Angular HttpErrorResponse shapes
    if (typeof e.error === 'string') return e.error;
    if (e.error?.title) return e.error.title;
    if (e.error?.message) return e.error.message;
    if (e.status) return `${e.status} ${e.statusText || ''}`.trim();
    try { return JSON.stringify(e.error ?? e); } catch { return String(e); }
  }

  reload(){
    this.error = '';
    this.issuesApi.byProject(this.projectId).subscribe({
      next: x => this.issues = x,
      error: e => { this.error = this.formatErr(e); console.error('byProject failed', e); }
    });
  }

  add(){
    const priorityMap: Record<string, number> = { Low:0, Medium:1, High:2, Critical:3 };
    const typeMap: Record<string, number> = { Task:0, Bug:1, Story:2, Epic:3 };
    if(!this.projectId){ this.error='No project selected'; return; }
    this.issuesApi.create({
      projectId: this.projectId,
      title: this.title,
      description: this.desc,
      priority: priorityMap['Medium'],
      type: typeMap['Task']
    }).subscribe({
      next: _ => { this.title=''; this.desc=''; this.reload(); },
      error: e => { this.error = this.formatErr(e); console.error('create issue failed', e); }
      
    });
  }

  move(i: Issue, status: string){
    this.issuesApi.setStatus(i.id, status).subscribe({
      next: u => i.status = u.status,
      error: e => { this.error = this.formatErr(e); console.error('setStatus failed', e); }
    });
  }

  list(col: string){ return this.issues.filter(i => i.status === col); }
  trackById(_i:number, it:Issue){ return it.id; }
}
