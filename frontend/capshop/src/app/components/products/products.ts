import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product, Category } from '../../models/product.model';

@Component({
  selector: 'app-products',
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  categories: Category[] = [];
  selectedCategory = 0;
  searchQuery = '';
  minPrice: number | null = null;
  maxPrice: number | null = null;
  sortBy = 'default';
  loading = true;
  toastMsg = '';

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.productService.getCategories().subscribe({
      next: (cats) => {
        this.categories = [...cats];
        this.cdr.detectChanges();
      }
    });

    this.route.queryParams.subscribe(params => {
      if (params['category']) {
        this.selectedCategory = +params['category'];
        this.loadByCategory(+params['category']);
      } else if (params['search']) {
        this.searchQuery = params['search'];
        this.loadProducts();
      } else {
        this.loadProducts();
      }
    });
  }

currentPage = 1;
pageSize = 12;
totalPages = 1;
totalProducts = 0;

 loadProducts() {
  this.loading = true;
  this.productService.getProducts(this.currentPage, this.pageSize).subscribe({
    next: (res) => {
      this.products = [...res.data];
      this.totalPages = res.totalPages;
      this.totalProducts = res.total;
      this.applyFilters();
      this.loading = false;
      this.cdr.detectChanges();
    },
    error: () => {
      this.loading = false;
      this.cdr.detectChanges();
    }
  });
}



changePage(page: number) {
  if (page < 1 || page > this.totalPages) return;
  this.currentPage = page;
  this.loadProducts();
  window.scrollTo(0, 0);
}


getPages(): number[] {
  const pages = [];
  const start = Math.max(1, this.currentPage - 2);
  const end = Math.min(this.totalPages, this.currentPage + 2);
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
}
  loadByCategory(categoryId: number) {
    this.loading = true;
    this.productService.getProductsByCategory(categoryId).subscribe({
      next: (products) => {
        this.products = [...products];
        this.applyFilters();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  filterByCategory(categoryId: number) {
    this.selectedCategory = categoryId;
    if (categoryId === 0) {
      this.loadProducts();
    } else {
      this.loadByCategory(categoryId);
    }
  }

  onSearch() {
    this.applyFilters();
    this.cdr.detectChanges();
  }

  applyFilters() {
    let result = [...this.products];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      result = result.filter(p =>
        p.name.toLowerCase().includes(q) ||
        p.description.toLowerCase().includes(q)
      );
    }

    if (this.minPrice !== null) {
      result = result.filter(p => p.price >= this.minPrice!);
    }

    if (this.maxPrice !== null) {
      result = result.filter(p => p.price <= this.maxPrice!);
    }

    if (this.sortBy === 'price-asc') {
      result.sort((a, b) => a.price - b.price);
    } else if (this.sortBy === 'price-desc') {
      result.sort((a, b) => b.price - a.price);
    } else if (this.sortBy === 'name') {
      result.sort((a, b) => a.name.localeCompare(b.name));
    }

    this.filteredProducts = result;
    this.cdr.detectChanges();
  }

  resetFilters() {
    this.searchQuery = '';
    this.minPrice = null;
    this.maxPrice = null;
    this.sortBy = 'default';
    this.selectedCategory = 0;
    this.loadProducts();
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

  viewProduct(id: number) {
    this.router.navigate(['/products', id]);
  }

  // ✅ Fixed - $event aur product dono accept karta hai
  addToCart(event: Event, product: Product) {
    event.stopPropagation(); // ✅ Card click se conflict nahi hoga
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

  // ✅ Added - onHover method
  onHover(event: MouseEvent, isHover: boolean) {
    const card = (event.currentTarget as HTMLElement);
    if (isHover) {
      card.style.transform = 'translateY(-4px)';
      card.style.boxShadow = '0 8px 24px rgba(0,0,0,0.12)';
    } else {
      card.style.transform = 'translateY(0)';
      card.style.boxShadow = '0 2px 8px rgba(0,0,0,0.06)';
    }
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