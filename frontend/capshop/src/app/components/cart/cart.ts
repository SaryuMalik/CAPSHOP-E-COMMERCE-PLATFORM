import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
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
  placing = false;
  shippingAddress = '';
  successMsg = '';
  errorMsg = '';

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadCart();
  }

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
    return this.cartItems.reduce((sum, item) =>
      sum + (item.productPrice * item.quantity), 0);
  }

  getProductEmoji(name: string): string {
    const n = name.toLowerCase();
    if (n.includes('phone') || n.includes('iphone')) return '📱';
    if (n.includes('laptop') || n.includes('computer')) return '💻';
    if (n.includes('book')) return '📚';
    if (n.includes('shirt') || n.includes('cloth')) return '👕';
    return '📦';
  }

  removeItem(productId: number) {
    this.cartService.removeFromCart(productId).subscribe({
      next: () => {
        this.loadCart();
      }
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
  getEmoji(name: string): string {
  const n = name.toLowerCase();
  if (n.includes('phone') || n.includes('iphone')) return '📱';
  if (n.includes('laptop') || n.includes('computer')) return '💻';
  if (n.includes('book')) return '📚';
  if (n.includes('shirt') || n.includes('cloth')) return '👕';
  return '📦';
}

  placeOrder() {
    if (!this.shippingAddress.trim()) {
      this.errorMsg = 'Please enter shipping address!';
      this.cdr.detectChanges();
      return;
    }

    this.placing = true;
    this.errorMsg = '';
    this.cdr.detectChanges();

    const orderData = {
      shippingAddress: this.shippingAddress,
      items: this.cartItems.map(item => ({
        productId: item.productId,
        productName: item.productName,
        price: item.productPrice,
        quantity: item.quantity
      }))
    };

    this.orderService.placeOrder(orderData).subscribe({
      next: () => {
        this.placing = false;
        this.successMsg = 'Order placed successfully! 🎉';
        this.clearCart();
        this.cdr.detectChanges();
      },
      error: () => {
        this.placing = false;
        this.errorMsg = 'Failed to place order. Try again!';
        this.cdr.detectChanges();
      }
    });
  }
}