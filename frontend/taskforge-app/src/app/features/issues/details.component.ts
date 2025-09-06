import { Component } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIf, NgFor } from '@angular/common';
import { IssuesService } from '../../core/api/issues.service';
import { DatePipe } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-issue-details',
  imports: [FormsModule, NgIf, NgFor, RouterLink, DatePipe],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class DetailsComponent {
id = ''; issue: any = null; comments: any[] = [];
  error = ''; saving = false;

statusList = ['Backlog','Selected','InProgress','Review','Done'];
  priorityList = ['Low','Medium','High','Critical'];
  typeList = ['Task','Bug','Story','Epic'];

constructor(private route: ActivatedRoute, private api: IssuesService) {}
  ngOnInit(){
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.load();
  }
  private load(){
    this.api.get(this.id).subscribe({
      next: i => { this.issue = i; this.loadComments(); },
      error: e => this.error = e?.error?.title ?? 'Failed to load issue'
    });
  }
  private loadComments(){
    this.api.listComments(this.id).subscribe({
      next: c => this.comments = c
    });
  }
  save(){
    this.saving = true;
    const dto = {
      title: this.issue.title,
      description: this.issue.description,
      status: this.issue.status,
      priority: this.issue.priority,
      type: this.issue.type
    };
    this.api.update(this.id, dto).subscribe({
      next: i => { this.issue = i; this.saving = false; },
      error: e => { this.error = e?.error ?? 'Save failed'; this.saving = false; }
    });
  }
  addComment(body: string){
    if(!body.trim()) return;
    this.api.addComment(this.id, body).subscribe({
      next: _ => this.loadComments()
    });
  }
}
