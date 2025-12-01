import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { DeploymentsPageRoutingModule } from './deployments-routing.module';
import { DeploymentsPage } from './deployments.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    DeploymentsPageRoutingModule
  ],
  declarations: [DeploymentsPage]
})
export class DeploymentsPageModule {}
