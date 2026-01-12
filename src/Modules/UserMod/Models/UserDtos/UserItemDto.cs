using Entity.UserMod;
namespace UserMod.Models.UserDtos;
/// <summary>
/// 用户ItemDto
/// </summary>
/// <see cref="Entity.UserMod.User"/>
public class UserItemDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [MaxLength(50)]
    public string? UserName { get; set; }
    /// <summary>
    /// 昵称
    /// </summary>
    [MaxLength(50)]
    public string? NickName { get; set; }
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    
}
