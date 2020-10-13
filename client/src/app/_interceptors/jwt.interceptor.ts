import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: User;
    // 'take(1)' allows for the subscription to end once one thing (user oject) is returned
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      currentUser = user;
    });

    if (currentUser) { // if currentUser then clone request and add currentUser to it
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${ currentUser.token }`
        }
      })
    }
    
    return next.handle(request);
  }
}
