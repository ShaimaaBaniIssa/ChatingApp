import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountServive: AccountService) { }
  // before sending the request
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.accountServive.currentUser$.pipe(take(1)) //get only one , no need for unsubsecribe
      .subscribe({
        next: (user) => {
          if (user) {
            request = request.clone({
              setHeaders: {
                Authorization: 'Bearer ' + user.token
              }
            })

          }
        }
      })
    // Add the Interceptor to the app module

    return next.handle(request);
  }
}
