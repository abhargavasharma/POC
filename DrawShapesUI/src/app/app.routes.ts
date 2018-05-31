import { Routes } from '@angular/router';
import { DrawShapeComponent } from './components/draw-shape/draw-shape.component';

export const routes:Routes = [
    {path:'', component: DrawShapeComponent}
    // {path:'**', component: PageNotFoundComponent}
];