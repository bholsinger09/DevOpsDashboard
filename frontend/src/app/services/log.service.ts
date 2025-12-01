import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface SystemLog {
  id: number;
  level: 'Debug' | 'Info' | 'Warning' | 'Error' | 'Critical';
  source: string;
  message: string;
  stackTrace?: string;
  timestamp: Date;
  additionalData?: string;
}

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private apiUrl = `${environment.apiUrl}/logs`;

  constructor(private http: HttpClient) {}

  getLogs(level?: string, source?: string, limit?: number): Observable<SystemLog[]> {
    let params: any = {};
    if (level) params.level = level;
    if (source) params.source = source;
    if (limit) params.limit = limit.toString();
    
    const options = Object.keys(params).length > 0 ? { params } : {};
    return this.http.get<SystemLog[]>(this.apiUrl, options);
  }

  getLog(id: number): Observable<SystemLog> {
    return this.http.get<SystemLog>(`${this.apiUrl}/${id}`);
  }

  createLog(log: Partial<SystemLog>): Observable<SystemLog> {
    return this.http.post<SystemLog>(this.apiUrl, log);
  }

  deleteLog(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  cleanupOldLogs(daysOld: number = 30): Observable<any> {
    return this.http.delete(`${this.apiUrl}/cleanup`, { params: { daysOld: daysOld.toString() } });
  }

  getStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/stats`);
  }
}
