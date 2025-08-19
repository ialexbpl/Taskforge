import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor } from '@angular/common';
import { Router } from '@angular/router';
import { ProjectsService } from '../../core/api/projects.service';

@Component({
  standalone: true,
  selector: 'app-projects-list',
  imports: [FormsModule, NgFor],
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent {
projects:any[]=[]; name=''; key='';
  constructor(private projectsApi: ProjectsService, private router: Router) {}
  ngOnInit(){ this.projectsApi.list().subscribe(x=>this.projects=x); }
  create(){ this.projectsApi.create(this.name, this.key).subscribe(x=>{ this.projects.push(x); this.name=''; this.key=''; }); }
  openBoard(p:any){ this.router.navigate(['/board', p.id]); }
}
