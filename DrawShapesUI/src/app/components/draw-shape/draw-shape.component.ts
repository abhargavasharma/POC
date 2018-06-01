import { Component, OnInit }   from '@angular/core';
import { ShapesApi } from '../../api/ShapesApi/shapesApi';
import 'rxjs/Rx';
import { map } from "rxjs/operators";
 import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-draw-shape',
  templateUrl: './draw-shape.component.html',
  styleUrls: ['./draw-shape.component.css']
})
export class DrawShapeComponent{
  freeTextvalue="Draw a rectangle with a width of 250 and a height of 400";
  showErrors: boolean;

  name:string = '';
  width: number = 0;
  height: number = 0;
  radius: number = 0;
  side: number = 0;
  lines: any = [ [[100, 0], [0, 100]], [[100, 200], [100, 100]], [[100,200], [200,100]], [[100,100], [100,0]] ];
  shapesClass: string = '';

  constructor(public shapesApi : ShapesApi) { }

  public myResult:any;

  ngOnInit() {
     
  }

  public Clear = () => {
    this.freeTextvalue = "";
    this.name = "";
  }

  public generateImage = () => { 
    this.name = "";

    try {
        if (this.freeTextvalue) { 
          var results = null;
              this.shapesApi.getImageDetails(this.freeTextvalue)
              .then(resp => {
                results = resp.json();
                console.log(resp);

                this.name = results.name;

                if(results.width)
                  this.width = results.width;

                if(results.height)
                  this.height = results.height;
                
                if(results.radius)
                this.radius = results.radius;

                if(results.side && results.name == "Square")
                {
                  this.width = results.side;
                  this.height = results.side;
                }

                if(results.name == "Equilateral" || results.name == 'Isosceles' || results.name == 'Scalene')
                {
                  this.lines = [ [[results.height, 0], [0, results.height]], [[results.height, results.width], [results.height, results.height]], [[results.height,results.width], [results.width,results.height]], [[results.height,results.height], [results.height,0]] ];
                }

                if(results.name == "Oval")
                {
                  this.shapesClass = "oval center";
                }

                if(results.name == "Parallelogram")
                {
                  this.shapesClass = "parallelogram center";
                }

              })
              .catch((resp) => {
                console.log(resp);
              });
          
        } else {
          this.showErrors = true;
          return null;
        }
    } 
    catch (e) {
        console.log(e);
    }
  }


}
