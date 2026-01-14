import { Blog } from '../entity/blog.model';
import { BlogCategory } from '../entity/blog-category.model';

/**
 * 博客与分类关系表
 */
export interface BlogCategoryRelation {
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** updatedTime */
  updatedTime: Date;
  /** isDeleted */
  isDeleted: boolean;
  /** tenantId */
  tenantId: string;
  /** 博客ID */
  blogId: string;
  /** 博客 */
  blog: Blog;
  /** 分类ID */
  categoryId: string;
  /** 博客分类 */
  category: BlogCategory;
}
