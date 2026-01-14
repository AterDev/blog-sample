import { BaseService } from '../base.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BlogFilterDto } from '../models/blog-mod/blog-filter-dto.model';
import { PageList } from '../models/perigon/page-list.model';
import { BlogItemDto } from '../models/blog-mod/blog-item-dto.model';
import { BlogAddDto } from '../models/blog-mod/blog-add-dto.model';
import { Blog } from '../models/entity/blog.model';
import { BlogUpdateDto } from '../models/blog-mod/blog-update-dto.model';
import { BlogDetailDto } from '../models/blog-mod/blog-detail-dto.model';
/**
 * 博客
 */
@Injectable({ providedIn: 'root' })
export class BlogService extends BaseService {
  /**
   * list 博客 with page ✅
   * @param data BlogFilterDto
   */
  list(data: BlogFilterDto): Observable<PageList<BlogItemDto>> {
    const _url = `/api/Blog/filter`;
    return this.request<PageList<BlogItemDto>>('post', _url, data);
  }
  /**
   * Add 博客 ✍️
   * @param data BlogAddDto
   */
  add(data: BlogAddDto): Observable<Blog> {
    const _url = `/api/Blog`;
    return this.request<Blog>('post', _url, data);
  }
  /**
   * Update 博客 ✍️
   * @param id
   * @param data BlogUpdateDto
   */
  update(id: string, data: BlogUpdateDto): Observable<boolean> {
    const _url = `/api/Blog/${id}`;
    return this.request<boolean>('patch', _url, data);
  }
  /**
   * Get 博客 Detail ✍️
   * @param id
   */
  detail(id: string): Observable<BlogDetailDto> {
    const _url = `/api/Blog/${id}`;
    return this.request<BlogDetailDto>('get', _url);
  }
  /**
   * Delete 博客 ✍️
   * @param id
   */
  delete(id: string): Observable<boolean> {
    const _url = `/api/Blog/${id}`;
    return this.request<boolean>('delete', _url);
  }
}