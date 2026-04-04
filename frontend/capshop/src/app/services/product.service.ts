import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { Product, Category } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private api = environment.catalogApiUrl;

  constructor(private http: HttpClient) {}

  // ✅ page aur pageSize parameters add kiye
  getProducts(page: number = 1, pageSize: number = 12): Observable<any> {
    return this.http.get<any>(`${this.api}/products?page=${page}&pageSize=${pageSize}`);
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.api}/products/${id}`);
  }

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.api}/products/category/${categoryId}`);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.api}/categories`);
  }

  createProduct(product: any): Observable<Product> {
    return this.http.post<Product>(`${this.api}/products`, product);
  }

  updateProduct(id: number, product: any): Observable<Product> {
    return this.http.put<Product>(`${this.api}/products/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.api}/products/${id}`);
  }
}