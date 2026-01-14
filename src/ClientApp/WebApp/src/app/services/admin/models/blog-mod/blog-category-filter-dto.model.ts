/**
 * 博客分类FilterDto
 */
export interface BlogCategoryFilterDto {
  /** pageIndex */
  pageIndex?: number | null;
  /** pageSize */
  pageSize?: number | null;
  /** orderBy */
  orderBy?: Record<string, boolean> | null;
  /** 分类名称 */
  name?: string | null;
}
