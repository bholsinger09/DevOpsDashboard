import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { GithubPageRoutingModule } from './github-routing.module';
import { GithubPage } from './github.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    GithubPageRoutingModule
  ],
  declarations: [GithubPage]
})
export class GithubPageModule {}
