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
  /** 作者ID */
  authorId?: string | null;
  /** categoryId */
  categoryId?: string | null;
}
