export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string;
  categoryId: number;
  categoryName: string;
  isActive: boolean;
}

export interface Category {
  id: number;
  name: string;
  description: string;
}