import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  email = '';
  password = '';
  errorMsg = '';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onLogin() {
    this.loading = true;
    this.errorMsg = '';
    this.cdr.detectChanges();

    this.authService.login({ email: this.email, password: this.password })
      .subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.loading = false;
          this.errorMsg = err.error?.message || 'Invalid email or password!';
          this.cdr.detectChanges();
        }
      });
  }
}