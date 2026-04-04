import { Routes } from '@angular/router';
import { Home } from './components/home/home';
import { Login } from './components/login/login';
import { Register } from './components/register/register';
import { Products } from './components/products/products';
import { ProductDetail } from './components/product-detail/product-detail';
import { Cart } from './components/cart/cart';
import { Orders } from './components/orders/orders';
import { authGuard } from './guards/auth.guard';
import { Admin } from './components/admin/admin';
import { ForgotPassword } from './components/forgot-password/forgot-password';
import { ResetPassword } from './components/reset-password/reset-password';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'products', component: Products },
  { path: 'products/:id', component: ProductDetail },
  { path: 'forgot-password', component: ForgotPassword },
{ path: 'reset-password', component: ResetPassword },
  { path: 'admin', component: Admin, canActivate: [authGuard] },
  { path: 'cart', component: Cart, canActivate: [authGuard] },
  { path: 'orders', component: Orders, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];