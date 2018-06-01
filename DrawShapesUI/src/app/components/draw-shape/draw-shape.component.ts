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
  freeTextvalue="Draw a oval with a width of 250 and a length of 400";
  errorMessage: string;

  name:string = '';
  width: number = 0;
  height: number = 0;
  radius: number = 0;
  side: number = 0;
  lines: any = [ [[100, 0], [0, 100]], [[100, 200], [100, 100]], [[100,200], [200,100]], [[100,100], [100,0]] ];
  shapesClass: any;

  constructor(public shapesApi : ShapesApi) { }

  public myResult:any;

  ngOnInit() {
     
  }

  public Clear = () => {
    this.freeTextvalue = "";
    this.clearDetails();
  }

  clearDetails(){
    this.name = "";
    this. errorMessage = "";
  }

  public generateImage = () => { 
    this.clearDetails();
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
                  this.shapesClass = { 'height': results.length + 'px',
                    'width': results.width + 'px',
                    'background-color': '#555',
                    'border-radius': '50%'
                  }
                }

                if(results.name == "Parallelogram")
                {
                  this.shapesClass = { 	'width': results.width + 'px',
                    'height': results.height + 'px',
                    'transform': 'skew(20deg)',
                    'background': '#555'
                }
                }

              })
              .catch((resp) => {
                this.errorMessage = "Provided input is invalid, corret format is: Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)";
                console.log(resp);
              });
          
        } else {
          this.errorMessage = "Provided input is invalid, corret format is: Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)";
          return null;
        }
    } 
    catch (e) {
        console.log(e);
    }
  }


}
