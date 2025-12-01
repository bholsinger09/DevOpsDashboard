import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  
  public serverStatusUpdate$ = new BehaviorSubject<any>(null);
  public deploymentNotification$ = new BehaviorSubject<any>(null);
  public logNotification$ = new BehaviorSubject<any>(null);
  public gitHubUpdate$ = new BehaviorSubject<any>(null);

  constructor() {}

  public startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
        this.registerHandlers();
      })
      .catch((err) => console.error('Error starting SignalR connection:', err));

    this.hubConnection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
    });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR reconnected');
    });

    this.hubConnection.onclose(() => {
      console.log('SignalR connection closed');
      setTimeout(() => this.startConnection(), 5000);
    });
  }

  private registerHandlers(): void {
    this.hubConnection.on('ReceiveServerStatusUpdate', (data: any) => {
      console.log('Server status update:', data);
      this.serverStatusUpdate$.next(data);
    });

    this.hubConnection.on('ReceiveDeploymentNotification', (data: any) => {
      console.log('Deployment notification:', data);
      this.deploymentNotification$.next(data);
    });

    this.hubConnection.on('ReceiveLogNotification', (data: any) => {
      console.log('Log notification:', data);
      this.logNotification$.next(data);
    });

    this.hubConnection.on('ReceiveGitHubUpdate', (data: any) => {
      console.log('GitHub update:', data);
      this.gitHubUpdate$.next(data);
    });
  }

  public stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
}
