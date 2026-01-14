import { User } from '../entity/user.model';
import { BlogCategory } from '../entity/blog-category.model';
import { BlogCategoryRelation } from '../entity/blog-category-relation.model';

/**
 * 博客
 */
export interface Blog {
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
  /** 标题 */
  title: string;
  /** 内容 */
  content: string;
  /** 作者ID */
  authorId: string;
  /** 用户 */
  author: User;
  /** 博客分类集合（用于EF Core关联查询） */
  blogCategories: BlogCategory[];
  /** 博客分类关系列表 */
  blogCategoryRelations: BlogCategoryRelation[];
}
