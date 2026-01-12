using Entity.UserMod;
namespace UserMod.Models.UserDtos;
/// <summary>
/// 用户UpdateDto
/// </summary>
/// <see cref="Entity.UserMod.User"/>
public class UserUpdateDto
{
    [MaxLength(100)]
    public string? Password { get; set; }
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
    
}
