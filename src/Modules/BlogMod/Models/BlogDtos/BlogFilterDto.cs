using Entity.BlogMod;
namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客FilterDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogFilterDto : FilterBase
{
    /// <summary>
    /// 作者ID
    /// </summary>
    public Guid? AuthorId { get; set; }
    public Guid? CategoryId { get; set; }
}
