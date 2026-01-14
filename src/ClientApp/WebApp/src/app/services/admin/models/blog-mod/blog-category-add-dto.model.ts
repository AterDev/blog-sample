/**
 * 博客分类AddDto
 */
export interface BlogCategoryAddDto {
  /** 分类名称 */
  name: string;
  /** 描述 */
  description?: string | null;
}
