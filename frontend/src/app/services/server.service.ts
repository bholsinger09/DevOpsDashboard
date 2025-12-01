import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Server {
  id: number;
  name: string;
  url: string;
  description?: string;
  status: 'Unknown' | 'Online' | 'Offline' | 'Degraded';
  lastChecked?: Date;
  uptimePercentage: number;
  responseTimeMs: number;
  createdAt: Date;
  updatedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class ServerService {
  private apiUrl = `${environment.apiUrl}/servers`;

  constructor(private http: HttpClient) {}

  getServers(): Observable<Server[]> {
    return this.http.get<Server[]>(this.apiUrl);
  }

  getServer(id: number): Observable<Server> {
    return this.http.get<Server>(`${this.apiUrl}/${id}`);
  }

  createServer(server: Partial<Server>): Observable<Server> {
    return this.http.post<Server>(this.apiUrl, server);
  }

  updateServer(id: number, server: Server): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, server);
  }

  deleteServer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/stats`);
  }
}
