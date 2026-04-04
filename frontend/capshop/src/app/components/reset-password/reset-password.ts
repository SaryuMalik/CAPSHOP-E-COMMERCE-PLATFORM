import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {
  email = '';
  otp = '';
  newPassword = '';
  confirmPassword = '';
  showPassword = false;
  showConfirmPassword = false;
  loading = false;
  errorMsg = '';
  successMsg = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      this.cdr.detectChanges();
    });
  }

  onSubmit() {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMsg = 'Passwords do not match!';
      this.cdr.detectChanges();
      return;
    }

    if (this.newPassword.length < 6) {
      this.errorMsg = 'Password must be at least 6 characters!';
      this.cdr.detectChanges();
      return;
    }

    this.loading = true;
    this.errorMsg = '';
    this.cdr.detectChanges();

    this.authService.resetPassword(this.email, this.otp, this.newPassword).subscribe({
      next: (res) => {
        this.loading = false;
        this.successMsg = res.message + ' Redirecting to login...';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.loading = false;
        this.errorMsg = err.error?.message || 'Reset failed!';
        this.cdr.detectChanges();
      }
    });
  }
}