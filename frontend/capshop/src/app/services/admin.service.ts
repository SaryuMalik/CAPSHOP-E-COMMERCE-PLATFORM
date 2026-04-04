import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private adminApi = `${environment.adminApiUrl}/admin`;  // ✅ Admin API
  private orderApi = `${environment.orderApiUrl}`;

  constructor(private http: HttpClient) {}

  // Products
  getProducts(): Observable<any[]> {
    return this.http.get<any>(`${this.adminApi}/products`).pipe(
      map(response => Array.isArray(response) ? response : response.data || [])  // ✅ data extract
    );
  }

  createProduct(data: any): Observable<any> {
    return this.http.post(`${this.adminApi}/products`, data);
  }

  updateProduct(id: number, data: any): Observable<any> {
    return this.http.put(`${this.adminApi}/products/${id}`, data);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.adminApi}/products/${id}`);
  }

  // Categories
  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.adminApi}/categories`);
  }

  // Orders
  getOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.adminApi}/orders`);
  }

  updateOrderStatus(id: number, status: string, paymentStatus?: string): Observable<any> {
    return this.http.put(`${this.adminApi}/orders/${id}/status`, { status, paymentStatus });
  }
}