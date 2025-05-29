import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    username: string;
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
export class AuthService {
    private apiUrl = 'http://localhost:5063/api/Auth';
    private tokenKey = 'auth_token';
    private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());

    constructor(private http: HttpClient) { }

    login(credentials: LoginRequest): Observable<AuthResponse> {
        return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, credentials)
            .pipe(
                map(response => {
                    if (response.success && response.data) {
                        return response.data;
                    }
                    throw new Error(response.message || 'Login failed');
                }),
                tap(response => {
                    this.setToken(response.token);
                    this.isAuthenticatedSubject.next(true);
                })
            );
    }

    logout(): void {
        localStorage.removeItem(this.tokenKey);
        this.isAuthenticatedSubject.next(false);
    }

    getToken(): string | null {
        const token = localStorage.getItem(this.tokenKey);
        return token;
    }

    private setToken(token: string): void {
        localStorage.setItem(this.tokenKey, token);
    }

    isAuthenticated(): Observable<boolean> {
        return this.isAuthenticatedSubject.asObservable();
    }

    private hasValidToken(): boolean {
        const token = this.getToken();
        return !!token;
    }
} 