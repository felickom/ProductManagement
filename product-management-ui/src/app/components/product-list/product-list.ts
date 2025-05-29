import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProductService, ProductSearchParams } from '../../services/product';
import { Product } from '../../models/product';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductList implements OnInit, OnDestroy {
  products: Product[] = [];
  searchParams: ProductSearchParams = {
    name: '',
    minPrice: null,
    maxPrice: null
  };
  private searchSubject = new Subject<void>();

  constructor(private productService: ProductService) {
    this.searchSubject
      .pipe(debounceTime(300))
      .subscribe(() => this.loadProducts());
  }

  ngOnInit() {
    this.loadProducts();
  }

  ngOnDestroy() {
    this.searchSubject.complete();
  }

  onSearch() {
    this.searchSubject.next();
  }

  onPriceInput(value: string | number | null, field: 'minPrice' | 'maxPrice') {
    if (typeof value === 'string') {
      this.searchParams[field] = value === '' ? null : Number(value);
    } else {
      this.searchParams[field] = value;
    }
    this.onSearch();
  }

  loadProducts() {
    // Clean up search params before sending
    const cleanParams: ProductSearchParams = {};

    if (this.searchParams.name?.trim()) {
      cleanParams.name = this.searchParams.name.trim();
    }

    if (this.searchParams.minPrice !== null) {
      cleanParams.minPrice = this.searchParams.minPrice;
    }

    if (this.searchParams.maxPrice !== null) {
      cleanParams.maxPrice = this.searchParams.maxPrice;
    }

    this.productService.getProducts(cleanParams).subscribe({
      next: (products) => {
        this.products = products;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        // TODO: Add proper error handling/user notification
      }
    });
  }

  deleteProduct(id: number) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.products = this.products.filter(p => p.id !== id);
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          // TODO: Add proper error handling/user notification
        }
      });
    }
  }

  clearSearch() {
    this.searchParams = {
      name: '',
      minPrice: null,
      maxPrice: null
    };
    this.loadProducts();
  }
}
