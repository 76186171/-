import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

import { BlogRoutingModule } from './blog-routing.module';
import { MaterialModule } from '../shared/material/material.module';
import { BlogAppComponent } from './blog-app.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';

@NgModule({
  imports: [
    CommonModule,
    BlogRoutingModule,
    HttpClientModule,
    MaterialModule
  ],
  declarations: [
    BlogAppComponent,
    SidenavComponent,
    ToolbarComponent
  ]
})
export class BlogModule { }
