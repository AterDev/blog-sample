using Entity.UserMod;
namespace UserMod.Models.UserDtos;
/// <summary>
/// 用户AddDto
/// </summary>
/// <see cref="Entity.UserMod.User"/>
public class UserAddDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [MaxLength(50)]
    public string UserName { get; set; } = default!;
    /// <summary>
    /// 密码
    /// </summary>
    [MaxLength(100)]
    public string Password { get; set; } = default!;
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
