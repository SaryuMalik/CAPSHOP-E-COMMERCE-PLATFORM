export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  total: number;
}

export interface Order {
  id: number;
  userId: string;
  userEmail: string;
  orderDate: string;
  status: string;
  paymentStatus: string;
  totalAmount: number;
  shippingAddress: string;
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  shippingAddress: string;
  items: {
    productId: number;
    productName: string;
    price: number;
    quantity: number;
  }[];
}