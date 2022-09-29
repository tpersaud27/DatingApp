import { ToastrService } from 'ngx-toastr';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { ValueConverter } from '@angular/compiler/src/render3/view/template';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  // We use the router here because for certain types of errors we want to relocate the user to a different page
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        // If there is an error
        if (error) {
          switch (error.status) {
            case 400:
              // Check for the error based on username and password
              if (error.error.errors) {
                const modelStateErrors = [];
                for (const key in error.error.errors) {
                  // If the error exists
                  if (error.error.errors[key]) {
                    // Add error to array
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                // Throw errors back to the components
                // We will display a list of validation errors for the user
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong!');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  }
}
