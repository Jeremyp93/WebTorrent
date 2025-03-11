import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs';
import { inject } from '@angular/core';

import { AuthService } from './auth.service';

export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    tap({error: (errorResponse: HttpErrorResponse) => {
      if (errorResponse.status === 401) {
        authService.logout();
        return;
      }
    }})
  );
}