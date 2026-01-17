/**
 * 博客FilterDto
 */
export interface BlogFilterDto {
  /** pageIndex */
  pageIndex?: number | null;
  /** pageSize */
  pageSize?: number | null;
  /** orderBy */
  orderBy?: Record<string, boolean> | null;
  /** categoryId */
  categoryId?: string | null;
}
