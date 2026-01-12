using Entity.BlogMod;
namespace BlogMod.Models.BlogCategoryDtos;
/// <summary>
/// 博客分类DetailDto
/// </summary>
/// <see cref="Entity.BlogMod.BlogCategory"/>
public class BlogCategoryDetailDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [MaxLength(60)]
    public string? Name { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
    public Guid TenantId { get; set; }
    
}
