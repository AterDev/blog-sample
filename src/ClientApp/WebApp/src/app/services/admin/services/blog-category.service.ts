import { BaseService } from '../base.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BlogCategoryFilterDto } from '../models/blog-mod/blog-category-filter-dto.model';
import { PageList } from '../models/perigon/page-list.model';
import { BlogCategoryItemDto } from '../models/blog-mod/blog-category-item-dto.model';
import { BlogCategoryAddDto } from '../models/blog-mod/blog-category-add-dto.model';
import { BlogCategory } from '../models/entity/blog-category.model';
import { BlogCategoryUpdateDto } from '../models/blog-mod/blog-category-update-dto.model';
import { BlogCategoryDetailDto } from '../models/blog-mod/blog-category-detail-dto.model';
/**
 * 博客分类
 */
@Injectable({ providedIn: 'root' })
export class BlogCategoryService extends BaseService {
  /**
   * list 博客分类 with page ✍️
   * @param data BlogCategoryFilterDto
   */
  list(data: BlogCategoryFilterDto): Observable<PageList<BlogCategoryItemDto>> {
    const _url = `/api/BlogCategory/filter`;
    return this.request<PageList<BlogCategoryItemDto>>('post', _url, data);
  }
  /**
   * Add 博客分类 ✍️
   * @param data BlogCategoryAddDto
   */
  add(data: BlogCategoryAddDto): Observable<BlogCategory> {
    const _url = `/api/BlogCategory`;
    return this.request<BlogCategory>('post', _url, data);
  }
  /**
   * Update 博客分类 ✍️
   * @param id
   * @param data BlogCategoryUpdateDto
   */
  update(id: string, data: BlogCategoryUpdateDto): Observable<boolean> {
    const _url = `/api/BlogCategory/${id}`;
    return this.request<boolean>('patch', _url, data);
  }
  /**
   * Get 博客分类 Detail ✍️
   * @param id
   */
  detail(id: string): Observable<BlogCategoryDetailDto> {
    const _url = `/api/BlogCategory/${id}`;
    return this.request<BlogCategoryDetailDto>('get', _url);
  }
  /**
   * Delete 博客分类 ✍️
   * @param id
   */
  delete(id: string): Observable<boolean> {
    const _url = `/api/BlogCategory/${id}`;
    return this.request<boolean>('delete', _url);
  }
}