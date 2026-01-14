import { inject, Injectable } from '@angular/core';
import { BlogService } from './services/blog.service';
import { BlogCategoryService } from './services/blog-category.service';
import { UserService } from './services/user.service';
@Injectable({
  providedIn: 'root'
})
export class AdminClient {
  /** 博客 */
  public blog = inject(BlogService);
  /** 博客分类 */
  public blogCategory = inject(BlogCategoryService);
  /** 用户 */
  public user = inject(UserService);
}
