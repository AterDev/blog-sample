using Entity.BlogMod;
namespace BlogMod.Models.BlogCategoryDtos;
/// <summary>
/// 博客分类FilterDto
/// </summary>
/// <see cref="Entity.BlogMod.BlogCategory"/>
public class BlogCategoryFilterDto : FilterBase
{
    /// <summary>
    /// 分类名称
    /// </summary>
    [MaxLength(60)]
    public string? Name { get; set; }
    
}
