import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, map, of } from 'rxjs';
import { Product } from '../models/product';

export interface ProductSearchParams {
  name?: string;
  minPrice?: number | null;
  maxPrice?: number | null;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  statusCode: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'http://localhost:5063/api/Products'; // Updated to match backend controller case

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

    return this.http.get<ApiResponse<Product[]>>(this.apiUrl, { params })
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          }
          throw new Error(response.message || 'Failed to fetch products');
        })
      );
  }

  // Get a single product by ID
  getProduct(id: number): Observable<Product> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/${id}`)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          }
          throw new Error(response.message || 'Failed to fetch product');
        })
      );
  }

  // Create a new product
  createProduct(product: Product): Observable<Product> {
    return this.http.post<ApiResponse<Product>>(this.apiUrl, product)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            return response.data;
          }
          throw new Error(response.message || 'Failed to create product');
        })
      );
  }

  // Update an existing product
  updateProduct(id: number, product: Product): Observable<Product> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, product, { observe: 'response' })
      .pipe(
        map(response => {
          // Handle 204 No Content response (success with no body)
          if (response.status === 204) {
            return product; // Return the product that was sent
          }
          
          // Handle regular ApiResponse
          const body = response.body;
          if (body && body.success && body.data) {
            return body.data;
          }
          
          throw new Error((body && body.message) || 'Failed to update product');
        })
      );
  }

  // Delete a product
  deleteProduct(id: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map(response => {
          // Handle 204 No Content response (success with no body)
          if (response.status === 204) {
            return;
          }
          
          // Handle regular ApiResponse
          const body = response.body;
          if (body && body.success) {
            return;
          }
          
          throw new Error((body && body.message) || 'Failed to delete product');
        })
      );
  }
}
