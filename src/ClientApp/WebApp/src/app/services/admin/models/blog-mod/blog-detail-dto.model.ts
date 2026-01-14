import { BlogCategoryItemDto } from '../blog-mod/blog-category-item-dto.model';

/**
 * 博客DetailDto
 */
export interface BlogDetailDto {
  /** 标题 */
  title?: string | null;
  /** 内容 */
  content?: string | null;
  /** 作者ID */
  authorId?: string | null;
  /** 分类信息 */
  categories?: BlogCategoryItemDto[] | null;
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** updatedTime */
  updatedTime: Date;
  /** tenantId */
  tenantId: string;
}
