import { Component, OnInit } from '@angular/core';
import { LogService, SystemLog } from '../../services/log.service';

@Component({
  selector: 'app-logs',
  templateUrl: './logs.page.html',
  styleUrls: ['./logs.page.scss'],
})
export class LogsPage implements OnInit {
  logs: SystemLog[] = [];
  filterLevel: string = '';

  constructor(private logService: LogService) {}

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs() {
    this.logService.getLogs(this.filterLevel || undefined, undefined, 100)
      .subscribe(logs => this.logs = logs);
  }

  filterByLevel(level: string) {
    this.filterLevel = level;
    this.loadLogs();
  }

  doRefresh(event: any) {
    this.loadLogs();
    setTimeout(() => event.target.complete(), 1000);
  }

  getLevelColor(level: string): string {
    const colors: any = {
      Error: 'danger',
      Critical: 'danger',
      Warning: 'warning',
      Info: 'primary',
      Debug: 'medium'
    };
    return colors[level] || 'medium';
  }

  getLevelIcon(level: string): string {
    const icons: any = {
      Error: 'close-circle',
      Critical: 'alert-circle',
      Warning: 'warning',
      Info: 'information-circle',
      Debug: 'bug'
    };
    return icons[level] || 'document';
  }
}
