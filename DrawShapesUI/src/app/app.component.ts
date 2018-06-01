import { Component,OnInit,AfterViewInit,ViewChild,Inject } from '@angular/core';
import { SpinnerService, SpinnerContent} from './shared/spinner.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements  AfterViewInit {
  title = 'app';
  showSpinner:boolean=false;
  spinnerContent:SpinnerContent;
  constructor(@Inject(SpinnerService)private spinnerService:SpinnerService){
  }

  public ngAfterViewInit() {
    
    this.spinnerService.status.subscribe((val:boolean)=>{
      this.showSpinner = val;
    });
    this.spinnerService.content.subscribe((content:SpinnerContent)=>{
      this.spinnerContent = content;
    });

}
}
