import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProductService, ProductSearchParams } from '../../services/product';
import { Product } from '../../models/product';
import { NotificationService } from '../../services/notification';
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
  isLoading = false;

  constructor(
    private productService: ProductService,
    private notificationService: NotificationService
  ) {
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
    this.isLoading = true;
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
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.notificationService.showError('Failed to load products. Please try again.');
        this.isLoading = false;
      }
    });
  }

  deleteProduct(id: number, productName: string) {
    if (confirm(`Are you sure you want to delete "${productName}"?`)) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.products = this.products.filter(p => p.id !== id);
          this.notificationService.showSuccess(`Product "${productName}" deleted successfully.`);
        },
        error: (error) => {
          console.error('Error deleting product:', error);
          this.notificationService.showError('Failed to delete product. Please try again.');
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
