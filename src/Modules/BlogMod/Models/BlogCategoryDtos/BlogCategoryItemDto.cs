using Entity.BlogMod;
namespace BlogMod.Models.BlogCategoryDtos;
/// <summary>
/// 博客分类ItemDto
/// </summary>
/// <see cref="Entity.BlogMod.BlogCategory"/>
public class BlogCategoryItemDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [MaxLength(60)]
    public string? Name { get; set; }
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    
}
