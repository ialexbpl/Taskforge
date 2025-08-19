import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgFor } from '@angular/common';
import { IssuesService } from '../../core/api/issues.service';

type Issue = { id:string, title:string, status:string };

@Component({
 standalone: true,
  selector: 'app-board',
  imports: [FormsModule, NgFor],
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss']
})
export class BoardComponent {
  projectId = '';
  issues: Issue[] = [];
  columns = ['Backlog','Selected','InProgress','Review','Done'];
  title=''; desc='';

  constructor(private route: ActivatedRoute, private issuesApi: IssuesService) {}
  ngOnInit(){
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.reload();
  }
  reload(){ this.issuesApi.byProject(this.projectId).subscribe(x=> this.issues = x); }
  add(){
    this.issuesApi.create({
      projectId: this.projectId,
      title: this.title,
      description: this.desc,
      priority: 'Medium',
      type: 'Task'
    }).subscribe(_=>{ this.title=''; this.desc=''; this.reload(); });
  }
  move(i: Issue, status: string){
    this.issuesApi.setStatus(i.id, status).subscribe(u=> i.status = u.status);
}
  list(col: string): Issue[] {
    return this.issues.filter(i => i.status === col);
  }
    trackById(_idx: number, item: Issue) { return item.id; }
}
