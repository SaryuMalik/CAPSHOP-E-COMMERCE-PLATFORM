import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class Orders implements OnInit {
  orders: Order[] = [];
  loading = true;

  constructor(
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        this.orders = [...orders];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  getStatusClass(status: string): string {
    return `badge badge-${status.toLowerCase()}`;
  }

  getPaymentClass(status: string): string {
    return `badge badge-${status.toLowerCase()}`;
  }

  getProductEmoji(name: string): string {
    const n = name.toLowerCase();
    if (n.includes('phone') || n.includes('iphone')) return '📱';
    if (n.includes('laptop') || n.includes('computer')) return '💻';
    if (n.includes('book')) return '📚';
    if (n.includes('shirt') || n.includes('cloth')) return '👕';
    return '📦';
  }
}