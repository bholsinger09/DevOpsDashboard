import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./pages/dashboard/dashboard.module').then(m => m.DashboardPageModule)
  },
  {
    path: 'servers',
    loadChildren: () => import('./pages/servers/servers.module').then(m => m.ServersPageModule)
  },
  {
    path: 'github',
    loadChildren: () => import('./pages/github/github.module').then(m => m.GithubPageModule)
  },
  {
    path: 'deployments',
    loadChildren: () => import('./pages/deployments/deployments.module').then(m => m.DeploymentsPageModule)
  },
  {
    path: 'logs',
    loadChildren: () => import('./pages/logs/logs.module').then(m => m.LogsPageModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
