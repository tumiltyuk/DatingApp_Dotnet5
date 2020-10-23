import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          switch (error.status) {
            
            // 400 - Bad Request - Generate list of Validation Errors (array)
            case 400:
              if (error.error.errors) {
                const modelStateErrors = [];

                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }

                throw modelStateErrors.flat();
              }
              // 400 - Bad Request - Single Bad Request
              else if (typeof(error.error) === 'object') {
                this.toastr.error(error.statusText, error.status);
              } else {
                this.toastr.error(error.error, error.status);
              }
            break;

            // 401 - Unauthorised -
            case 401:
              this.toastr.error(error.statusText, error.status); 
            break;

            // 404 - Not Found -
            case 404: 
              // redirect to not found page
              this.router.navigateByUrl('/not-found');
            break;

            // 500 - Server Error -
            case 500:
              // redirect and pass details from response 
              const navigationExtras: NavigationExtras = { state: { error: error.error }};
              this.router.navigateByUrl('/server-error', navigationExtras);
            break;

            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
            break;
          }
        }
        return throwError(error);
      })
    )
  }
}
