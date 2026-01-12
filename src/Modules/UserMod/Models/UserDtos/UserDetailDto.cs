using Entity.UserMod;
namespace UserMod.Models.UserDtos;
/// <summary>
/// 用户DetailDto
/// </summary>
/// <see cref="Entity.UserMod.User"/>
public class UserDetailDto
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
    /// <summary>
    /// 头像URL
    /// </summary>
    [MaxLength(500)]
    public string? Avatar { get; set; }
    [Key]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedTime { get; set; } = DateTimeOffset.UtcNow;
}
