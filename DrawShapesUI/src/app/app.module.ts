import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { DrawShapeComponent } from './components/draw-shape/draw-shape.component';
import { NgvasModule, tweens, hitAreas } from "ngvas";
import { environment } from '../environments/environment';
import { ShapesApi } from './api/ShapesApi/shapesApi';
import { HttpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';//TODO:angular.json/styles -> should use relative path..  node_modules/bootstrap/dist/css/bootstrap.min.css
import {FormsModule} from '@angular/forms'
import { SpinnerService } from './shared/spinner.service'

@NgModule({
  declarations: [
    AppComponent,
    DrawShapeComponent
  ],
  imports: [
    BrowserModule,
    NgvasModule,
    HttpModule,
    NgbModule,
    FormsModule,
    RouterModule,
    RouterModule.forRoot(routes)
  ],
  providers: [SpinnerService, ShapesApi],
  bootstrap: [AppComponent]
})
export class AppModule { }
