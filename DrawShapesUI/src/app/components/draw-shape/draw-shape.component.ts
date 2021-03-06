import { Component, OnInit }   from '@angular/core';
import { ShapesApi } from '../../api/ShapesApi/shapesApi';
import 'rxjs/Rx';
import { map } from "rxjs/operators";
import { Observable } from 'rxjs/Observable';
import { SpinnerService } from '../../shared/spinner.service'

@Component({
  selector: 'app-draw-shape',
  templateUrl: './draw-shape.component.html',
  styleUrls: ['./draw-shape.component.css']
})
export class DrawShapeComponent{
  freeTextvalue="Draw a pentagon with a side length of 200";
  errorMessage: string;

  name:string = '';
  width: number = 0;
  height: number = 0;
  radius: number = 0;
  side: number = 0;
  lines: any = [ [[100, 0], [0, 100]], [[100, 200], [100, 100]], [[100,200], [200,100]], [[100,100], [100,0]] ];
  shapesClass: any;
  multiSidedShapesCss : any;

  constructor(public shapesApi : ShapesApi, private loader: SpinnerService) { }

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
    this.shapesClass = "";
    this.multiSidedShapesCss = "";
  }

    public generateImage = () => { 
      this.clearDetails();
      try {
          if (this.freeTextvalue) { 
            this.loader.show();

            // this.shapesApi.getImageDetails(this.freeTextvalue)
            // .then(resp => {
            //   console.log(resp);
            //   this.responseProcessing(resp);
            //   this.loader.hide();
            // })
            // .catch((resp) => {
            //   this.errorMessage = "Provided input is invalid, corret format is: Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)";
            //   console.log(resp);
            //   this.loader.hide();
            // });

            this.shapesApi.getImageDetails(this.freeTextvalue)
            .subscribe(resp => 
              { 
                console.log(resp.json());
                this.responseProcessing(resp);
                this.loader.hide();
              },
              error => {
                this.errorMessage = "Provided input is invalid, corret format is: Draw a(n) <shape> with a(n) <measurement> of <amount> (and a(n)< measurement > of<amount>)";
                this.loader.hide();
                console.log(error);
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

    responseProcessing(resp:any)
    {
      var results = resp.json();
      this.name = results.name;

        if(results.width)
          this.width = results.width;

        if(results.height)
          this.height = results.height;
        
        if(results.radius && results.name == "Circle")
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
                               'border-radius': '50%',
                               'margin':'auto'
                             }
        }

        if(results.name == "Parallelogram")
        {
          this.shapesClass = { 	'width': results.width + 'px',
                                'height': results.height + 'px',
                                'transform': 'skew(20deg)',
                                'background': '#555',
                                'margin':'auto'
                              }
        }

        if(results.name == "Pentagon")
        {
          this.shapesClass = {
              'pentagon.width':  results.side + 'px',
              'pentagon:before.border-width': '0 '+results.side/2+'px 35px'
          };
          this.width = results.side;
          this.multiSidedShapesCss = "pentagon";
        }

        if(results.name == "Hexagon")
        {
          this.shapesClass = {
              'hexagon.width':  results.side + 'px'
          };
          this.width = results.side;
          this.multiSidedShapesCss = "hexagon";
        }

        if(results.name == "Octagon")
        {
          this.shapesClass = {
              'octagon.width':  results.side + 'px'
          };
          this.width = results.side;
          this.multiSidedShapesCss = "octagon";
        }

        if(results.name == "Heptagon")
        {
          this.shapesClass = {
              'heptagon.width':  results.side + 'px'
          };
          this.width = results.side;
          this.multiSidedShapesCss = "heptagon";
        }
    }

}
