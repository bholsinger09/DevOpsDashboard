import { Component, OnInit } from '@angular/core';
import { DeploymentService, Deployment } from '../../services/deployment.service';
import { AlertController } from '@ionic/angular';

@Component({
  selector: 'app-deployments',
  templateUrl: './deployments.page.html',
  styleUrls: ['./deployments.page.scss'],
})
export class DeploymentsPage implements OnInit {
  deployments: Deployment[] = [];

  constructor(
    private deploymentService: DeploymentService,
    private alertController: AlertController
  ) {}

  ngOnInit() {
    this.loadDeployments();
  }

  loadDeployments() {
    this.deploymentService.getDeployments(50).subscribe(deployments => {
      this.deployments = deployments;
    });
  }

  async addDeployment() {
    const alert = await this.alertController.create({
      header: 'New Deployment',
      inputs: [
        { name: 'environment', type: 'text', placeholder: 'Environment (e.g., Production)' },
        { name: 'version', type: 'text', placeholder: 'Version (e.g., v1.2.3)' },
        { name: 'deployedBy', type: 'text', placeholder: 'Deployed By' },
        { name: 'notes', type: 'textarea', placeholder: 'Notes (optional)' }
      ],
      buttons: [
        { text: 'Cancel', role: 'cancel' },
        {
          text: 'Create',
          handler: (data) => {
            if (data.environment && data.version && data.deployedBy) {
              this.deploymentService.createDeployment(data).subscribe(() => this.loadDeployments());
            }
          }
        }
      ]
    });
    await alert.present();
  }

  doRefresh(event: any) {
    this.loadDeployments();
    setTimeout(() => event.target.complete(), 1000);
  }

  getStatusColor(status: string): string {
    const colors: any = {
      Success: 'success',
      Failed: 'danger',
      InProgress: 'primary',
      Pending: 'warning',
      Cancelled: 'medium'
    };
    return colors[status] || 'medium';
  }

  getStatusIcon(status: string): string {
    const icons: any = {
      Success: 'checkmark-circle',
      Failed: 'close-circle',
      InProgress: 'sync-circle',
      Pending: 'time',
      Cancelled: 'ban'
    };
    return icons[status] || 'help-circle';
  }
}
