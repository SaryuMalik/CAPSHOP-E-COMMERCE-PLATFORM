import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-detail',
  imports: [CommonModule, RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css'
})
export class ProductDetail implements OnInit {
  product: Product | null = null;
  loading = true;
  quantity = 1;
  toastMsg = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef   // ✅ Added
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productService.getProductById(+id).subscribe({
        next: (product) => {
          this.product = { ...product };  // ✅ Spread
          this.loading = false;
          this.cdr.detectChanges();       // ✅ Force update
        },
        error: () => {
          this.loading = false;
          this.cdr.detectChanges();
        }
      });
    }
  }

  increaseQty() {
    if (this.product && this.quantity < this.product.stock) {
      this.quantity++;
    }
  }

  decreaseQty() {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    if (!this.product) return;

    this.cartService.addToCart(this.product.id, this.quantity).subscribe({
      next: () => {
        this.showToast('✅ Cart mein add ho gaya!');
        this.cartService.getCart().subscribe({
          next: (cart: any) => {
            this.cartService.updateCartCount(cart.items?.length || 0);
            this.cdr.detectChanges();   // ✅ Force update
          }
        });
      },
      error: () => {
        this.showToast('❌ Error! Dobara try karo.');
      }
    });
  }

  showToast(msg: string) {
    this.toastMsg = msg;
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toastMsg = '';
      this.cdr.detectChanges();   // ✅ Force update
    }, 3000);
  }

  goBack() {
    this.router.navigate(['/products']);
  }

  getStockPercent(): number {
    if (!this.product) return 0;
    // Cap at 100 units as "full stock" for visual bar
    return Math.min((this.product.stock / 100) * 100, 100);
  }
}