import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { Order, PlaceOrderRequest } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private api = `${environment.orderApiUrl}/orders`;

  constructor(private http: HttpClient) {}

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.api);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.api}/${id}`);
  }
  getAllOrders(): Observable<Order[]> {
  return this.http.get<Order[]>(`${this.api}/all`);
}

  placeOrder(order: PlaceOrderRequest): Observable<any> {
    return this.http.post(this.api, order);
  }

  updateOrderStatus(id: number, status: string, paymentStatus?: string): Observable<any> {
    return this.http.put(`${this.api}/${id}/status`, { status, paymentStatus });
  }
}