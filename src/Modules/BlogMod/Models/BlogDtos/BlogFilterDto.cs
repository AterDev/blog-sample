namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客FilterDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogFilterDto : FilterBase
{
    public Guid? CategoryId { get; set; }
}
