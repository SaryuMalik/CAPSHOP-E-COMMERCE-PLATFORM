import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environment/environment'
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private api = `${environment.authApiUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.api}/login`, data).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('user', JSON.stringify(res.user));
      })
    );
  }

  register(data: RegisterRequest): Observable<any> {
    return this.http.post(`${this.api}/register`, data);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }


  forgotPassword(email: string): Observable<any> {
  return this.http.post(`${this.api}/forgot-password`, { email });
  }

resetPassword(email: string, otp: string, newPassword: string): Observable<any> {
  return this.http.post(`${this.api}/reset-password`, { email, otp, newPassword });
  }

  getUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}