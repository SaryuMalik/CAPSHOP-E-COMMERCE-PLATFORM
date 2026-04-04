import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive, CommonModule, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {
  isLoggedIn = false;
  isAdmin = false;
  userName = '';
  cartCount = 0;
  isDark = false;
  searchQuery = '';

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.checkLoginStatus();
      this.cdr.detectChanges();
    });

    this.checkLoginStatus();

    this.cartService.cartCount$.subscribe(count => {
      this.cartCount = count;
      this.cdr.detectChanges();
    });
  }

  checkLoginStatus() {
    this.isLoggedIn = this.authService.isLoggedIn();
    const user = this.authService.getUser();
    if (user) {
      this.userName = user.firstName;
      this.isAdmin = user.role === 'Admin';
    } else {
      this.isAdmin = false;
    }
    this.cdr.detectChanges();
  }

  toggleTheme() {
    this.isDark = !this.isDark;
    if (this.isDark) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
    this.cdr.detectChanges();
  }

  onSearch() {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/products'], {
        queryParams: { search: this.searchQuery }
      });
      this.searchQuery = '';
    }
  }

  logout() {
    this.authService.logout();
    this.isLoggedIn = false;
    this.isAdmin = false;
    this.userName = '';
    this.router.navigate(['/']);
    this.cdr.detectChanges();
  }
}