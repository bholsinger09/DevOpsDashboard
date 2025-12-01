import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DeploymentsPage } from './deployments.page';

const routes: Routes = [
  {
    path: '',
    component: DeploymentsPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DeploymentsPageRoutingModule {}
