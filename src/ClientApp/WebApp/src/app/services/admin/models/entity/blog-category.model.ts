import { Blog } from '../entity/blog.model';
import { BlogCategoryRelation } from '../entity/blog-category-relation.model';

/**
 * 博客分类
 */
export interface BlogCategory {
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
  /** 分类名称 */
  name: string;
  /** userId */
  userId: string;
  /** 描述 */
  description?: string | null;
  /** 博客集合（用于EF Core关联查询） */
  blogs: Blog[];
  /** 博客分类关系列表 */
  blogCategoryRelations: BlogCategoryRelation[];
}
