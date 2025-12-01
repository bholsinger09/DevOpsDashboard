import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface GitHubIssue {
  id: number;
  gitHubId: number;
  title: string;
  description?: string;
  state: string;
  author: string;
  assignedTo?: string;
  url: string;
  createdAt: Date;
  updatedAt?: Date;
  closedAt?: Date;
  commentsCount: number;
  labels?: string;
}

export interface GitHubPullRequest {
  id: number;
  gitHubId: number;
  title: string;
  description?: string;
  state: string;
  author: string;
  url: string;
  createdAt: Date;
  updatedAt?: Date;
  mergedAt?: Date;
  closedAt?: Date;
  isMerged: boolean;
  sourceBranch?: string;
  targetBranch?: string;
  commentsCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class GithubService {
  private apiUrl = `${environment.apiUrl}/github`;

  constructor(private http: HttpClient) {}

  getIssues(state?: string): Observable<GitHubIssue[]> {
    const params = state ? { state } : {};
    return this.http.get<GitHubIssue[]>(`${this.apiUrl}/issues`, { params });
  }

  getPullRequests(state?: string): Observable<GitHubPullRequest[]> {
    const params = state ? { state } : {};
    return this.http.get<GitHubPullRequest[]>(`${this.apiUrl}/pullrequests`, { params });
  }

  syncGitHubData(): Observable<any> {
    return this.http.post(`${this.apiUrl}/sync`, {});
  }

  getStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/stats`);
  }
}
