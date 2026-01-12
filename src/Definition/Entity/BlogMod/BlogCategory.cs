using System.ComponentModel.DataAnnotations;
using Entity.UserMod;
using Microsoft.EntityFrameworkCore;

namespace Entity.BlogMod;

/// <summary>
/// 博客分类
/// </summary>
[Index(nameof(Name), IsUnique = true)]
public class BlogCategory : EntityBase
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [MaxLength(60)]
    public required string Name { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 博客集合（用于EF Core关联查询）
    /// </summary>
    public ICollection<Blog> Blogs { get; set; } = [];

    /// <summary>
    /// 博客分类关系列表
    /// </summary>
    public ICollection<BlogCategoryRelation> BlogCategoryRelations { get; set; } = [];
}
