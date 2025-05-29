import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    expiresIn?: number;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:5063/api/auth'; // Adjust this URL to match your backend auth endpoint
    private tokenKey = 'auth_token';
    private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());

    constructor(private http: HttpClient) { }

    login(credentials: LoginRequest): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
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
        return localStorage.getItem(this.tokenKey);
    }

    private setToken(token: string): void {
        localStorage.setItem(this.tokenKey, token);
    }

    isAuthenticated(): Observable<boolean> {
        return this.isAuthenticatedSubject.asObservable();
    }

    private hasValidToken(): boolean {
        const token = this.getToken();
        // You might want to add token expiration validation here
        return !!token;
    }
} 