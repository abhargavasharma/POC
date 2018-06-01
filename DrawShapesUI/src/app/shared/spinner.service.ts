import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface SpinnerContent {
    displayText:string
}

@Injectable()
export class SpinnerService {

  defaultContent:SpinnerContent = {displayText:"" };
  status:BehaviorSubject<boolean> = new BehaviorSubject(false);
  content: BehaviorSubject<SpinnerContent> = new BehaviorSubject(this.defaultContent);
  // or just `Subject` depending on your requirements'

  public show(displayText:string=null) {
      this.status.next(true);
      var newContent:SpinnerContent = {displayText:displayText};
      this.content.next(newContent);
  }
  public hide():void {
    this.status.next(false);
    this.content.next(this.defaultContent);
  }
}