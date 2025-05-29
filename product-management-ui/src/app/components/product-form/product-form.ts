import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product';
import { Product } from '../../models/product';
import { NotificationService } from '../../services/notification';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss'
})
export class ProductForm implements OnInit {
  product: Product = {
    name: '',
    description: '',
    price: 0
  };
  isEditMode = false;
  isSubmitting = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private productService: ProductService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.loadProduct(+id);
    }
  }

  loadProduct(id: number) {
    this.productService.getProduct(id).subscribe({
      next: (product) => {
        this.product = product;
      },
      error: (error) => {
        console.error('Error loading product:', error);
        this.notificationService.showError('Failed to load product details.');
        this.router.navigate(['/products']);
      }
    });
  }

  onSubmit() {
    if (this.isSubmitting) return;
    
    this.isSubmitting = true;
    const operation = this.isEditMode
      ? this.productService.updateProduct(this.product.id!, this.product)
      : this.productService.createProduct(this.product);

    operation.subscribe({
      next: () => {
        const message = this.isEditMode 
          ? `Product "${this.product.name}" updated successfully.` 
          : `Product "${this.product.name}" created successfully.`;
        this.notificationService.showSuccess(message);
        this.router.navigate(['/products']);
      },
      error: (error) => {
        console.error('Error saving product:', error);
        this.notificationService.showError('Failed to save product. Please try again.');
        this.isSubmitting = false;
      }
    });
  }
}
