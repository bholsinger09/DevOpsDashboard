import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Deployment {
  id: number;
  environment: string;
  version: string;
  status: 'Pending' | 'InProgress' | 'Success' | 'Failed' | 'Cancelled';
  deployedBy: string;
  startedAt: Date;
  completedAt?: Date;
  notes?: string;
  errorMessage?: string;
  durationSeconds?: number;
}

@Injectable({
  providedIn: 'root'
})
export class DeploymentService {
  private apiUrl = `${environment.apiUrl}/deployments`;

  constructor(private http: HttpClient) {}

  getDeployments(limit?: number): Observable<Deployment[]> {
    const params = limit ? { limit: limit.toString() } : {};
    return this.http.get<Deployment[]>(this.apiUrl, { params });
  }

  getDeployment(id: number): Observable<Deployment> {
    return this.http.get<Deployment>(`${this.apiUrl}/${id}`);
  }

  createDeployment(deployment: Partial<Deployment>): Observable<Deployment> {
    return this.http.post<Deployment>(this.apiUrl, deployment);
  }

  updateDeployment(id: number, deployment: Deployment): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, deployment);
  }

  getStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/stats`);
  }
}
