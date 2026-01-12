using Entity.BlogMod;
namespace BlogMod.Models.BlogCategoryDtos;
/// <summary>
/// 博客分类UpdateDto
/// </summary>
/// <see cref="Entity.BlogMod.BlogCategory"/>
public class BlogCategoryUpdateDto
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
    
}
