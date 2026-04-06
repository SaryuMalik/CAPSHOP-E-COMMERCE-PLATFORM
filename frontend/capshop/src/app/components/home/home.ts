import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product, Category } from '../../models/product.model';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  featuredProducts: Product[] = [];
  categories: Category[] = [];
  toastMsg = '';
  isLoggedIn = false;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.isLoggedIn = this.authService.isLoggedIn();
    this.productService.getProducts(1, 8).subscribe({  // ✅ page=1, pageSize=8
      next: (res) => {
        this.featuredProducts = [...(res.data || res).slice(0, 8)];  // ✅ data extract
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Products error:', err)
    });

    this.productService.getCategories().subscribe({
      next: (cats) => {
        this.categories = [...cats];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Categories error:', err)
    });
  }

  getCategoryIcon(name: string): string {
    const icons: any = {
      'Electronics': '💻',
      'Clothing': '👕',
      'Books': '📚',
      'Home & Kitchen': '🏠',
      'Sports': '⚽',
      'Beauty': '💄',
      'Toys': '🧸',
      'Furniture': '🛋️',
      'default': '🛍️'
    };
    return icons[name] || icons['default'];
  }

  goToCategory(id: number) {
    this.router.navigate(['/products'], { queryParams: { category: id } });
  }

  viewProduct(id: number) {
    this.router.navigate(['/products', id]);
  }

  onCatHover(event: MouseEvent, isEnter: boolean) {
    const el = event.currentTarget as HTMLElement;
    el.style.boxShadow = isEnter ? '0 4px 20px rgba(0,0,0,0.1)' : 'none';
  }

  onProductHover(event: MouseEvent, isEnter: boolean) {
    const el = event.currentTarget as HTMLElement;
    el.style.boxShadow = isEnter
      ? '0 8px 24px rgba(0,0,0,0.12)'
      : '0 2px 8px rgba(0,0,0,0.06)';
  }

  addToCart(event: Event, product: Product) {
    event.stopPropagation();
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.cartService.addToCart(product.id, 1).subscribe({
      next: () => {
        this.showToast('✅ Added to cart!');
        this.cartService.getCart().subscribe((cart: any) => {
          this.cartService.updateCartCount(cart.items?.length || 0);
          this.cdr.detectChanges();
        });
      },
      error: () => this.showToast('❌ Error! Try again.')
    });
  }

  showToast(msg: string) {
    this.toastMsg = msg;
    this.cdr.detectChanges();
    setTimeout(() => {
      this.toastMsg = '';
      this.cdr.detectChanges();
    }, 3000);
  }
}