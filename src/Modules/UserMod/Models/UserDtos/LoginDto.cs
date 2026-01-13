namespace UserMod.Models.UserDtos;

/// <summary>
/// 用户登录Dto
/// </summary>
public class LoginDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [MaxLength(50)]
    public required string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [MaxLength(100)]
    public required string Password { get; set; }
}
