import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  #authService = inject(AuthService);
  #route = inject(ActivatedRoute);
  #router = inject(Router);

  ngOnInit(): void {
    // Check for the 'token' parameter in the URL
    const token = this.#route.snapshot.queryParamMap.get('token');
    if (token) {
      this.#authService.validateToken(token).subscribe(() => this.#router.navigate(['/dashboard']));
    }
  }

  login = () => {
    this.#authService.loginWithGoogle();
  }
}
