using Entity.BlogMod;
namespace BlogMod.Models.BlogCategoryDtos;
/// <summary>
/// 博客分类AddDto
/// </summary>
/// <see cref="Entity.BlogMod.BlogCategory"/>
public class BlogCategoryAddDto
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [MaxLength(60)]
    public string Name { get; set; } = default!;
    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
}
