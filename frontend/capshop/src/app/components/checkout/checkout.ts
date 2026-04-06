import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CartItem } from '../../models/cart.model';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class Checkout implements OnInit {
  cartItems: CartItem[] = [];
  loading = true;
  placing = false;
  step = 1; // 1=Shipping, 2=Payment, 3=Success

  // Shipping
  firstName = '';
  lastName = '';
  email = '';
  phone = '';
  address = '';
  city = '';
  state = '';
  pincode = '';

  // Payment
  paymentMethod = 'card'; // card | upi | cod
  cardNumber = '';
  cardName = '';
  cardExpiry = '';
  cardCvv = '';
  upiId = '';

  errorMsg = '';
  orderId: number | null = null;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const user = this.authService.getUser();
    if (user) {
      this.firstName = user.firstName || '';
      this.lastName  = user.lastName  || '';
      this.email     = user.email     || '';
    }

    this.cartService.getCart().subscribe({
      next: (cart: any) => {
        this.cartItems = cart.items || [];
        this.loading = false;
        if (this.cartItems.length === 0) this.router.navigate(['/cart']);
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/cart']);
      }
    });
  }

  getTotal(): number {
    return this.cartItems.reduce((s, i) => s + i.productPrice * i.quantity, 0);
  }

  getShipping(): number { return this.getTotal() >= 499 ? 0 : 49; }
  getGrandTotal(): number { return this.getTotal() + this.getShipping(); }

  // Step 1 → 2
  proceedToPayment() {
    if (!this.firstName || !this.lastName || !this.address || !this.city || !this.pincode) {
      this.errorMsg = 'Please fill all required fields.';
      return;
    }
    this.errorMsg = '';
    this.step = 2;
    window.scrollTo(0, 0);
  }

  // Step 2 → place order
  placeOrder() {
    if (this.paymentMethod === 'card') {
      if (!this.cardNumber || !this.cardName || !this.cardExpiry || !this.cardCvv) {
        this.errorMsg = 'Please fill all card details.';
        return;
      }
    }
    if (this.paymentMethod === 'upi' && !this.upiId) {
      this.errorMsg = 'Please enter your UPI ID.';
      return;
    }

    this.errorMsg = '';
    this.placing = true;

    const shippingAddress = `${this.address}, ${this.city}, ${this.state} - ${this.pincode}`;

    const orderData = {
      shippingAddress,
      items: this.cartItems.map(i => ({
        productId: i.productId,
        productName: i.productName,
        price: i.productPrice,
        quantity: i.quantity
      }))
    };

    this.orderService.placeOrder(orderData).subscribe({
      next: (res: any) => {
        this.orderId = res.orderId;
        this.placing = false;
        this.step = 3;
        this.cartService.clearCart().subscribe();
        this.cartService.updateCartCount(0);
        window.scrollTo(0, 0);
        this.cdr.detectChanges();
      },
      error: () => {
        this.placing = false;
        this.errorMsg = 'Failed to place order. Please try again.';
        this.cdr.detectChanges();
      }
    });
  }

  formatCard(val: string): string {
    return val.replace(/\D/g, '').replace(/(.{4})/g, '$1 ').trim().slice(0, 19);
  }

  onCardInput(e: Event) {
    const el = e.target as HTMLInputElement;
    el.value = this.formatCard(el.value);
    this.cardNumber = el.value;
  }

  onExpiryInput(e: Event) {
    const el = e.target as HTMLInputElement;
    let v = el.value.replace(/\D/g, '');
    if (v.length >= 2) v = v.slice(0, 2) + '/' + v.slice(2, 4);
    el.value = v;
    this.cardExpiry = v;
  }
}
