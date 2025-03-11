import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AuthToken } from './models/token.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;  // Replace with your API URL
  #isAuthenticatedKey = 'authenticated_key';         // Key to store the token in localStorage

  constructor(private http: HttpClient, private router: Router) {}

  // Method to start OAuth login process
  loginWithGoogle(): void {
    // Redirect to OAuth provider, passing in your backend endpoint as the callback
    window.location.href = `${this.apiUrl}/google/login`;
  }

  // Method to handle the OAuth callback and store the token
  handleOAuthGoogleCallback(code: string): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.apiUrl}/google/callback`, { code }, { headers })
      .pipe(
        tap(() => localStorage.setItem(this.#isAuthenticatedKey, 'true')),
        catchError(this.handleError('handleOAuthCallback', []))
      );
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem(this.#isAuthenticatedKey);
  }

  // Method to log out
  logout(): void {
    localStorage.removeItem(this.#isAuthenticatedKey);
    this.http.post(`${this.apiUrl}/logout`, null).pipe(
      catchError(this.handleError('logout'))
    ).subscribe(() => this.router.navigate(['/auth/login']));
  }

  generateToken(): Observable<AuthToken | null> {
    return this.http.get<AuthToken>(`${this.apiUrl}/token/generate`)
    .pipe(
      catchError(this.handleError<AuthToken | null>('handleTokenLogin', null))
    );
  }

  validateToken(token: string) {
    return this.http.post(`${this.apiUrl}/token/validate`, {token})
    .pipe(
      tap(() => localStorage.setItem(this.#isAuthenticatedKey, 'true')),
      catchError(this.handleError('handleTokenLogin', []))
    );
  }

  // Error handler
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return of(result as T);
    };
  }
}