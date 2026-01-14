/**
 * 博客UpdateDto
 */
export interface BlogUpdateDto {
  /** 标题 */
  title?: string | null;
  /** 内容 */
  content?: string | null;
  /** 分类ID集合 */
  categoryIds?: string[] | null;
}
