using Entity.BlogMod;
namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客UpdateDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogUpdateDto
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
    /// 分类ID集合
    /// </summary>
    public List<Guid>? CategoryIds { get; set; }
}
