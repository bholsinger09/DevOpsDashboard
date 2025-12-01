import { Component, OnInit } from '@angular/core';
import { SignalRService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit {
  public appPages = [
    { title: 'Dashboard', url: '/dashboard', icon: 'speedometer' },
    { title: 'Servers', url: '/servers', icon: 'server' },
    { title: 'GitHub', url: '/github', icon: 'logo-github' },
    { title: 'Deployments', url: '/deployments', icon: 'rocket' },
    { title: 'Logs', url: '/logs', icon: 'document-text' },
  ];

  constructor(private signalRService: SignalRService) {}

  ngOnInit() {
    this.signalRService.startConnection();
  }
}
