using Entity.BlogMod;
using System.ComponentModel.DataAnnotations;
using BlogMod.Models.BlogCategoryDtos;

namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客DetailDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogDetailDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }
    /// <summary>
    /// 内容
    /// </summary>
    [MaxLength(50000)]
    public string? Content { get; set; }
    /// <summary>
    /// 作者ID
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// 分类信息
    /// </summary>
    public List<BlogCategoryItemDto>? Categories { get; set; }

    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
    public Guid TenantId { get; set; }
}
