import { Component, OnInit, OnDestroy } from '@angular/core';
import { ServerService } from '../../services/server.service';
import { GithubService } from '../../services/github.service';
import { DeploymentService } from '../../services/deployment.service';
import { LogService } from '../../services/log.service';
import { SignalRService } from '../../services/signalr.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.page.html',
  styleUrls: ['./dashboard.page.scss'],
})
export class DashboardPage implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  serverStats: any = {};
  githubStats: any = {};
  deploymentStats: any = {};
  logStats: any = {};

  recentDeployments: any[] = [];
  recentLogs: any[] = [];

  // Chart data
  serverChartData: any;
  deploymentChartData: any;

  constructor(
    private serverService: ServerService,
    private githubService: GithubService,
    private deploymentService: DeploymentService,
    private logService: LogService,
    private signalRService: SignalRService
  ) {}

  ngOnInit() {
    this.loadStats();
    this.setupRealtimeUpdates();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadStats() {
    this.serverService.getStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe(stats => {
        this.serverStats = stats;
        this.updateServerChart();
      });

    this.githubService.getStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe(stats => this.githubStats = stats);

    this.deploymentService.getStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe(stats => {
        this.deploymentStats = stats;
        this.updateDeploymentChart();
      });

    this.logService.getStats()
      .pipe(takeUntil(this.destroy$))
      .subscribe(stats => this.logStats = stats);

    this.deploymentService.getDeployments(5)
      .pipe(takeUntil(this.destroy$))
      .subscribe(deployments => this.recentDeployments = deployments);

    this.logService.getLogs(undefined, undefined, 5)
      .pipe(takeUntil(this.destroy$))
      .subscribe(logs => this.recentLogs = logs);
  }

  setupRealtimeUpdates() {
    this.signalRService.serverStatusUpdate$
      .pipe(takeUntil(this.destroy$))
      .subscribe(update => {
        if (update) {
          this.loadStats();
        }
      });

    this.signalRService.deploymentNotification$
      .pipe(takeUntil(this.destroy$))
      .subscribe(notification => {
        if (notification) {
          this.loadStats();
        }
      });
  }

  updateServerChart() {
    this.serverChartData = {
      labels: ['Online', 'Offline', 'Degraded'],
      datasets: [{
        data: [
          this.serverStats.online || 0,
          this.serverStats.offline || 0,
          this.serverStats.degraded || 0
        ],
        backgroundColor: ['#2dd36f', '#eb445a', '#ffc409']
      }]
    };
  }

  updateDeploymentChart() {
    this.deploymentChartData = {
      labels: ['Successful', 'Failed', 'In Progress'],
      datasets: [{
        data: [
          this.deploymentStats.successful || 0,
          this.deploymentStats.failed || 0,
          this.deploymentStats.inProgress || 0
        ],
        backgroundColor: ['#2dd36f', '#eb445a', '#3880ff']
      }]
    };
  }

  doRefresh(event: any) {
    this.loadStats();
    setTimeout(() => {
      event.target.complete();
    }, 1000);
  }

  getStatusColor(status: string): string {
    const colors: any = {
      Online: 'success',
      Offline: 'danger',
      Degraded: 'warning',
      Success: 'success',
      Failed: 'danger',
      InProgress: 'primary',
      Pending: 'medium'
    };
    return colors[status] || 'medium';
  }

  getLogLevelColor(level: string): string {
    const colors: any = {
      Error: 'danger',
      Critical: 'danger',
      Warning: 'warning',
      Info: 'primary',
      Debug: 'medium'
    };
    return colors[level] || 'medium';
  }
}
