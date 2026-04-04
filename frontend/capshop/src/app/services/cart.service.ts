import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../environment/environment';
import { Cart } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private api = `${environment.catalogApiUrl}/cart`;
  private cartCount = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCount.asObservable();

  constructor(private http: HttpClient) {}

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(this.api);
  }

  addToCart(productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.api}/add`, { productId, quantity });
  }

  removeFromCart(productId: number): Observable<any> {
    return this.http.delete(`${this.api}/remove/${productId}`);
  }

  clearCart(): Observable<any> {
    return this.http.delete(`${this.api}/clear`);
  }

  updateCartCount(count: number): void {
    this.cartCount.next(count);
  }
}