import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Product, Category } from '../../models/product.model';
import { Order } from '../../models/order.model';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin',
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class Admin implements OnInit {
  activeTab = 'products';
  products: Product[] = [];
  orders: Order[] = [];
  categories: Category[] = [];
  loadingProducts = true;
  loadingOrders = true;
  editingProduct: Product | null = null;
  formSuccess = '';
  formError = '';

  productForm = {
    name: '',
    description: '',
    price: 0,
    stock: 0,
    imageUrl: '',
    categoryId: 1
  };

  constructor(
    private adminService: AdminService,
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadProducts();
    this.loadOrders();
    this.loadCategories();
  }

  setTab(tab: string) {
    this.activeTab = tab;
    this.cdr.detectChanges();
  }

  loadProducts() {
    this.loadingProducts = true;
    this.adminService.getProducts().subscribe({
      next: (response: any) => {
        // ✅ Array bhi handle karo object bhi
        if (Array.isArray(response)) {
          this.products = [...response];
        } else {
          this.products = [...(response.data || response.products || response.items || [])];
        }
        this.loadingProducts = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Products error:', err);
        this.products = [];
        this.loadingProducts = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadOrders() {
    this.loadingOrders = true;
    this.adminService.getOrders().subscribe({
      next: (response: any) => {
        if (!response) {
          this.orders = [];
        } else if (Array.isArray(response)) {
          this.orders = [...response];
        } else {
          this.orders = [...(response.orders || response.items || response.data || [])];
        }
        this.loadingOrders = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Orders error:', err);
        this.orders = [];
        this.loadingOrders = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadCategories() {
    this.adminService.getCategories().subscribe({
      next: (cats) => {
        this.categories = [...cats];
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Categories error:', err)
    });
  }

  getTotalRevenue(): number {
    return this.orders.reduce((sum, o) => sum + o.totalAmount, 0);
  }

  getProductEmoji(name: string): string {
    const n = name.toLowerCase();
    if (n.includes('phone') || n.includes('iphone')) return '📱';
    if (n.includes('laptop') || n.includes('computer')) return '💻';
    if (n.includes('book')) return '📚';
    if (n.includes('shirt') || n.includes('cloth')) return '👕';
    return '📦';
  }

  editProduct(product: Product) {
    this.editingProduct = { ...product };
    this.productForm = {
      name: product.name,
      description: product.description,
      price: product.price,
      stock: product.stock,
      imageUrl: product.imageUrl,
      categoryId: product.categoryId
    };
    this.activeTab = 'add';
    this.cdr.detectChanges();
  }

  cancelEdit() {
    this.editingProduct = null;
    this.productForm = {
      name: '', description: '',
      price: 0, stock: 0, imageUrl: '', categoryId: 1
    };
    this.cdr.detectChanges();
  }

  saveProduct() {
    if (this.editingProduct) {
      this.adminService.updateProduct(this.editingProduct.id, this.productForm).subscribe({
        next: () => {
          this.formSuccess = 'Product updated!';
          this.formError = '';
          this.cancelEdit();
          this.loadProducts();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Update error:', err);
          this.formError = 'Update failed!';
          this.formSuccess = '';
          this.cdr.detectChanges();
        }
      });
    } else {
      this.adminService.createProduct(this.productForm).subscribe({
        next: () => {
          this.formSuccess = 'Product added!';
          this.formError = '';
          this.cancelEdit();
          this.loadProducts();
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('Create error:', err);
          this.formError = 'Add failed!';
          this.formSuccess = '';
          this.cdr.detectChanges();
        }
      });
    }
  }

  deleteProduct(id: number) {
    if (confirm('Delete karna chahte ho?')) {
      this.adminService.deleteProduct(id).subscribe({
        next: () => this.loadProducts(),
        error: (err) => console.error('Delete error:', err)
      });
    }
  }

  updateStatus(orderId: number, event: any, type: string) {
    const value = event.target.value;
    const order = this.orders.find(o => o.id === orderId);
    if (!order) return;

    const status = type === 'status' ? value : order.status;
    const paymentStatus = type === 'payment' ? value : order.paymentStatus;

    this.adminService.updateOrderStatus(orderId, status, paymentStatus).subscribe({
      next: () => this.loadOrders(),
      error: (err) => console.error('Status update error:', err)
    });
  }
}