export interface CartItem {
  id: number;
  productId: number;
  productName: string;
  productPrice: number;
  quantity: number;
}

export interface Cart {
  id: number;
  userId: string;
  items: CartItem[];
}