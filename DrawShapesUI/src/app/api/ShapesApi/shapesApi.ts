
import { Inject, Injectable }     from '@angular/core';
import { environment }            from '../../../environments/environment';
import { Http, Headers, Response }          from '@angular/http';
import { nullSafeIsEquivalent } from '@angular/compiler/src/output/output_ast';
import { delay } from 'rxjs/operators';
// Below modules will be used if suscription model is used.
// import 'rxjs/Rx';
// import { map } from "rxjs/operators";
// import { Observable } from 'rxjs/Observable';
/* tslint:disable:no-unused-variable member-ordering */


@Injectable()
export class ShapesApi {
    protected basePath =  environment.apiUrl + '/draw';
    constructor(private http: Http) {
        
    }

    // API: GET /getimagedetails/freetext
    public getImageDetails(value: string): any {
      var url= this.basePath + '/getimagedetails/'+value;
      return this.http.get(url)
      .toPromise();
    }
}
