/**
 * 博客分类DetailDto
 */
export interface BlogCategoryDetailDto {
  /** 分类名称 */
  name?: string | null;
  /** 描述 */
  description?: string | null;
  /** id */
  id: string;
  /** createdTime */
  createdTime: Date;
  /** updatedTime */
  updatedTime: Date;
  /** tenantId */
  tenantId: string;
}
