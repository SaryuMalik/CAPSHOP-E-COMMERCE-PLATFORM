import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css'
})
export class ForgotPassword {
  email = '';
  loading = false;
  errorMsg = '';
  successMsg = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onSubmit() {
    this.loading = true;
    this.errorMsg = '';
    this.successMsg = '';
    this.cdr.detectChanges();

    this.authService.forgotPassword(this.email).subscribe({
      next: (res) => {
        this.loading = false;
        this.successMsg = res.message;
        this.cdr.detectChanges();
        // Email ke saath reset page pe jao
        setTimeout(() => {
          this.router.navigate(['/reset-password'], {
            queryParams: { email: this.email }
          });
        }, 1500);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error?.message || 'Something went wrong!';
        this.cdr.detectChanges();
      }
    });
  }
}