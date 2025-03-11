import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-google-callback',
  standalone: true,
  imports: [CommonModule],
  template: '<p>Redirecting...</p>',
})
export class GoogleCallbackComponent implements OnInit {
  #route = inject(ActivatedRoute);
  #router = inject(Router);
  #authService = inject(AuthService);

  ngOnInit(): void {
    this.#route.queryParams.subscribe(params => {
      const code = params['code'];
      if (code) {
        // Call a service to send the authorization code to your backend
        this.#authService.handleOAuthGoogleCallback(code).subscribe({
          next: () => {
            this.#router.navigate([`/dashboard`]);
          },
          error: () => {
            this.#router.navigate(['/error']);
          }
        });
      } else {
        // Redirect to an error page or handle accordingly
        this.#router.navigate(['/error']);
      }
    });
  }
}

