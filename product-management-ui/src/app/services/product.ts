import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../models/product';

export interface ProductSearchParams {
  name?: string;
  minPrice?: number | null;
  maxPrice?: number | null;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5063/api/products'; // Adjust this URL to match your backend API

  constructor(private http: HttpClient) { }

  // Get all products with optional search parameters
  getProducts(searchParams?: ProductSearchParams): Observable<Product[]> {
    let params = new HttpParams();

    if (searchParams) {
      if (searchParams.name?.trim()) {
        params = params.set('name', searchParams.name.trim());
      }
      if (searchParams.minPrice !== undefined && searchParams.minPrice !== null) {
        params = params.set('minPrice', searchParams.minPrice.toString());
      }
      if (searchParams.maxPrice !== undefined && searchParams.maxPrice !== null) {
        params = params.set('maxPrice', searchParams.maxPrice.toString());
      }
    }

    return this.http.get<Product[]>(this.apiUrl, { params });
  }

  // Get a single product by ID
  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  // Create a new product
  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  // Update an existing product
  updateProduct(id: number, product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }

  // Delete a product
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
