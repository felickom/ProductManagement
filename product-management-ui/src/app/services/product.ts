import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable, map } from 'rxjs';
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
  private apiUrl = 'http://localhost:5063/api/Products';

  constructor(private http: HttpClient) { }

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
        map(response => this.handleResponse<Product[]>(response))
      );
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/${id}`)
      .pipe(
        map(response => this.handleResponse<Product>(response))
      );
  }

  createProduct(product: Product): Observable<Product> {
    return this.http.post<ApiResponse<Product>>(this.apiUrl, product)
      .pipe(
        map(response => this.handleResponse<Product>(response))
      );
  }

  updateProduct(id: number, product: Product): Observable<Product> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, product, { observe: 'response' })
      .pipe(
        map(response => {
          if (response.status === 204) {
            return product;
          }

          return this.handleResponse<Product>(response.body);
        })
      );
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map(response => {
          if (response.status === 204) {
            return;
          }
          
          if (response.body) {
            this.handleResponse<void>(response.body);
          }
          return;
        })
      );
  }

  private handleResponse<T>(response: ApiResponse<T>): T {
    if (response.success && response.data) {
      return response.data;
    }
    throw new Error(response.message || `Failed to process request`);
  }
}
