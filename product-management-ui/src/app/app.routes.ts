import { Routes } from '@angular/router';
import { ProductList } from './components/product-list/product-list';
import { ProductForm } from './components/product-form/product-form';
import { Login } from './components/login/login';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
    {
        path: 'login',
        component: Login
    },
    {
        path: '',
        redirectTo: 'products',
        pathMatch: 'full'
    },
    {
        path: 'products',
        component: ProductList,
        canActivate: [AuthGuard]
    },
    {
        path: 'products/new',
        component: ProductForm,
        canActivate: [AuthGuard]
    },
    {
        path: 'products/edit/:id',
        component: ProductForm,
        canActivate: [AuthGuard]
    }
];
