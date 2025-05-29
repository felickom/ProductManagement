import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './login.html',
    styleUrl: './login.scss'
})
export class Login {
    credentials = {
        username: '',
        password: ''
    };
    error = '';
    isLoading = false;

    constructor(
        private authService: AuthService,
        private router: Router
    ) { }

    onSubmit() {
        this.isLoading = true;
        this.error = '';

        this.authService.login(this.credentials).subscribe({
            next: () => {
                this.router.navigate(['/products']);
            },
            error: (error) => {
                this.error = error.error?.message || 'Invalid credentials';
                this.isLoading = false;
            }
        });
    }
} 