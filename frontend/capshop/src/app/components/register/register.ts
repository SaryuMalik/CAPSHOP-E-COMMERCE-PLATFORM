import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';
  errorMsg = '';
  successMsg = '';
  loading = false;
  showPassword = false;
  showConfirmPassword = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onRegister() {
    this.errorMsg = '';
    this.successMsg = '';

    // Validation
    if (this.password !== this.confirmPassword) {
      this.errorMsg = 'Passwords do not match!';
      this.cdr.detectChanges();
      return;
    }

    if (this.password.length < 6) {
      this.errorMsg = 'Password must be at least 6 characters!';
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();

    this.authService.register({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;
        this.successMsg = 'Account created! Redirecting to login...';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error?.message || 'Registration failed!';
        this.cdr.detectChanges();
      }
    });
  }
}