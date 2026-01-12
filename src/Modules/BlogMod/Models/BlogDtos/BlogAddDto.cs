using Entity.BlogMod;
namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客AddDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogAddDto
{
    /// <summary>
    /// 标题
    /// </summary>
    [MaxLength(200)]
    public string Title { get; set; } = default!;
    /// <summary>
    /// 内容
    /// </summary>
    [MaxLength(50000)]
    public string Content { get; set; } = default!;
    /// <summary>
    /// 作者ID
    /// </summary>
    public Guid AuthorId { get; set; } = default!;

    /// <summary>
    /// 分类ID集合（如果为空或null，则默认添加到"未分类"）
    /// </summary>
    public List<Guid> CategoryIds { get; set; } = [];
}
