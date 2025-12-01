import { Component, OnInit, OnDestroy } from '@angular/core';
import { ServerService, Server } from '../../services/server.service';
import { SignalRService } from '../../services/signalr.service';
import { Subject, takeUntil } from 'rxjs';
import { AlertController } from '@ionic/angular';

@Component({
  selector: 'app-servers',
  templateUrl: './servers.page.html',
  styleUrls: ['./servers.page.scss'],
})
export class ServersPage implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  servers: Server[] = [];
  filteredServers: Server[] = [];

  constructor(
    private serverService: ServerService,
    private signalRService: SignalRService,
    private alertController: AlertController
  ) {}

  ngOnInit() {
    this.loadServers();
    this.setupRealtimeUpdates();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadServers() {
    this.serverService.getServers()
      .pipe(takeUntil(this.destroy$))
      .subscribe(servers => {
        this.servers = servers;
        this.filteredServers = servers;
      });
  }

  setupRealtimeUpdates() {
    this.signalRService.serverStatusUpdate$
      .pipe(takeUntil(this.destroy$))
      .subscribe(update => {
        if (update) {
          const index = this.servers.findIndex(s => s.id === update.id);
          if (index !== -1) {
            this.servers[index] = { ...this.servers[index], ...update };
            this.filteredServers = [...this.servers];
          }
        }
      });
  }

  async addServer() {
    const alert = await this.alertController.create({
      header: 'Add Server',
      inputs: [
        { name: 'name', type: 'text', placeholder: 'Server Name' },
        { name: 'url', type: 'url', placeholder: 'Server URL' },
        { name: 'description', type: 'textarea', placeholder: 'Description (optional)' }
      ],
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        {
          text: 'Add',
          handler: (data) => {
            if (data.name && data.url) {
              this.serverService.createServer(data).subscribe(() => this.loadServers());
            }
          }
        }
      ]
    });
    await alert.present();
  }

  async deleteServer(server: Server) {
    const alert = await this.alertController.create({
      header: 'Confirm Delete',
      message: `Are you sure you want to delete ${server.name}?`,
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        {
          text: 'Delete',
          role: 'destructive',
          handler: () => {
            this.serverService.deleteServer(server.id).subscribe(() => this.loadServers());
          }
        }
      ]
    });
    await alert.present();
  }

  filterServers(event: any) {
    const query = event.target.value.toLowerCase();
    this.filteredServers = this.servers.filter(s => 
      s.name.toLowerCase().includes(query) || 
      s.url.toLowerCase().includes(query)
    );
  }

  getStatusColor(status: string): string {
    const colors: any = {
      Online: 'success',
      Offline: 'danger',
      Degraded: 'warning',
      Unknown: 'medium'
    };
    return colors[status] || 'medium';
  }

  getStatusIcon(status: string): string {
    const icons: any = {
      Online: 'checkmark-circle',
      Offline: 'close-circle',
      Degraded: 'warning',
      Unknown: 'help-circle'
    };
    return icons[status] || 'help-circle';
  }

  doRefresh(event: any) {
    this.loadServers();
    setTimeout(() => event.target.complete(), 1000);
  }
}
