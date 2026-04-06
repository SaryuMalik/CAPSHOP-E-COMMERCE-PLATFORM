import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { CartItem } from '../../models/cart.model';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart implements OnInit {
  cartItems: CartItem[] = [];
  loading = true;

  constructor(
    private cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() { this.loadCart(); }

  loadCart() {
    this.loading = true;
    this.cartService.getCart().subscribe({
      next: (cart: any) => {
        this.cartItems = cart.items || [];
        this.cartService.updateCartCount(this.cartItems.length);
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cartItems = [];
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  getTotal(): number {
    return this.cartItems.reduce((s, i) => s + i.productPrice * i.quantity, 0);
  }

  getShipping(): number { return this.getTotal() >= 499 ? 0 : 49; }
  getGrandTotal(): number { return this.getTotal() + this.getShipping(); }

  getEmoji(name: string): string {
    const n = name.toLowerCase();
    if (n.includes('phone') || n.includes('iphone')) return '📱';
    if (n.includes('laptop') || n.includes('macbook')) return '💻';
    if (n.includes('book')) return '📚';
    if (n.includes('shirt') || n.includes('cloth') || n.includes('hoodie')) return '👕';
    if (n.includes('shoe') || n.includes('nike')) return '👟';
    if (n.includes('watch')) return '⌚';
    if (n.includes('headphone') || n.includes('sony')) return '🎧';
    return '📦';
  }

  removeItem(productId: number) {
    this.cartService.removeFromCart(productId).subscribe({
      next: () => this.loadCart()
    });
  }

  clearCart() {
    this.cartService.clearCart().subscribe({
      next: () => {
        this.cartItems = [];
        this.cartService.updateCartCount(0);
        this.cdr.detectChanges();
      }
    });
  }

  proceedToCheckout() {
    this.router.navigate(['/checkout']);
  }
}
