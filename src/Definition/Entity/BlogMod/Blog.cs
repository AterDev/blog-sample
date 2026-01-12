using System.ComponentModel.DataAnnotations;
using Entity.UserMod;
using Microsoft.EntityFrameworkCore;

namespace Entity.BlogMod;

/// <summary>
/// 博客
/// </summary>
[Index(nameof(Title), IsUnique = true)]
[Index(nameof(AuthorId))]
public class Blog : EntityBase
{
    /// <summary>
    /// 标题
    /// </summary>
    [MaxLength(200)]
    public required string Title { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [MaxLength(50000)]
    public required string Content { get; set; }

    /// <summary>
    /// 作者ID
    /// </summary>
    public required Guid AuthorId { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    public User? Author { get; set; }

    /// <summary>
    /// 博客分类集合（用于EF Core关联查询）
    /// </summary>
    public ICollection<BlogCategory> BlogCategories { get; set; } = [];

    /// <summary>
    /// 博客分类关系列表
    /// </summary>
    public ICollection<BlogCategoryRelation> BlogCategoryRelations { get; set; } = [];
}
