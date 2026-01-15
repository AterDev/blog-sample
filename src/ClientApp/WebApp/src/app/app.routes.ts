import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { LayoutComponent } from './layout/layout';
import { Notfound } from './pages/notfound/notfound';
import { AuthGuard } from './share/auth.guard';
import { Index } from './pages/index';
import { BlogCategoryIndex } from './pages/blog-category/index';
import { BlogCategoryAdd } from './pages/blog-category/add';
import { BlogCategoryEdit } from './pages/blog-category/edit';
import { BlogCategoryDetail } from './pages/blog-category/detail';
import { BlogIndex } from './pages/blog/index';
import { BlogAdd } from './pages/blog/add';
import { BlogEdit } from './pages/blog/edit';
import { BlogDetail } from './pages/blog/detail';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'index', component: Index },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    canActivateChild: [AuthGuard],
    children: [
      { path: 'blog', component: BlogIndex },
      { path: 'blog/add', component: BlogAdd },
      { path: 'blog/edit/:id', component: BlogEdit },
      { path: 'blog/detail/:id', component: BlogDetail },

      { path: 'blogcategory', component: BlogCategoryIndex },
      { path: 'blogcategory/add', component: BlogCategoryAdd },
      { path: 'blogcategory/edit/:id', component: BlogCategoryEdit },
      { path: 'blogcategory/detail/:id', component: BlogCategoryDetail },
    ],
  },
  
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', component: Notfound },
];
