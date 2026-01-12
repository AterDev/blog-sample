using Microsoft.EntityFrameworkCore;

namespace Entity.BlogMod;

/// <summary>
/// 博客与分类关系表
/// </summary>
public class BlogCategoryRelation : EntityBase
{
    /// <summary>
    /// 博客ID
    /// </summary>
    public required Guid BlogId { get; set; }

    /// <summary>
    /// 博客
    /// </summary>
    [ForeignKey(nameof(BlogId))]
    public Blog? Blog { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public required Guid CategoryId { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    [ForeignKey(nameof(CategoryId))]
    public BlogCategory? Category { get; set; }
}
