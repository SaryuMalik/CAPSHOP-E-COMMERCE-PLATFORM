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
  maxStock = 0; // store original stock for bar calculation

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
      this.loadProduct(+id);
    }
  }

  loadProduct(id: number) {
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product = { ...product };
        if (this.maxStock === 0) this.maxStock = product.stock; // set once on first load
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
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
    const productId = this.product.id;

    this.cartService.addToCart(productId, this.quantity).subscribe({
      next: () => {
        this.showToast('✅ Cart mein add ho gaya!');
        this.cartService.getCart().subscribe({
          next: (cart: any) => {
            this.cartService.updateCartCount(cart.items?.length || 0);
            this.cdr.detectChanges();
          }
        });
        // refresh product to get latest stock from backend
        this.loadProduct(productId);
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
    if (!this.product || this.maxStock === 0) return 0;
    return Math.min((this.product.stock / this.maxStock) * 100, 100);
  }
}