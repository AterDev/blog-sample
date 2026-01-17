/**
 * 博客AddDto
 */
export interface BlogAddDto {
  /** 标题 */
  title: string;
  /** 内容 */
  content: string;
  /** 分类ID集合（如果为空或null，则默认添加到"未分类"） */
  categoryIds: string[];
}
