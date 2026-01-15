namespace BlogMod.Models.BlogDtos;
/// <summary>
/// 博客ItemDto
/// </summary>
/// <see cref="Entity.BlogMod.Blog"/>
public class BlogItemDto
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public string Title { get; set; } = null!;
    public string AuthorUserName { get; set; } = null!;
}
