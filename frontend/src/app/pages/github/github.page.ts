import { Component, OnInit } from '@angular/core';
import { GithubService, GitHubIssue, GitHubPullRequest } from '../../services/github.service';

@Component({
  selector: 'app-github',
  templateUrl: './github.page.html',
  styleUrls: ['./github.page.scss'],
})
export class GithubPage implements OnInit {
  segment = 'issues';
  issues: GitHubIssue[] = [];
  pullRequests: GitHubPullRequest[] = [];
  loading = false;

  constructor(private githubService: GithubService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    this.githubService.getIssues().subscribe(issues => {
      this.issues = issues;
      this.loading = false;
    });
    this.githubService.getPullRequests().subscribe(prs => {
      this.pullRequests = prs;
    });
  }

  syncGitHub() {
    this.loading = true;
    this.githubService.syncGitHubData().subscribe(() => {
      this.loadData();
    });
  }

  doRefresh(event: any) {
    this.loadData();
    setTimeout(() => event.target.complete(), 1000);
  }

  getStateColor(state: string): string {
    return state === 'open' ? 'warning' : 'success';
  }
}
