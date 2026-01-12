using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Entity.UserMod;

/// <summary>
/// 用户
/// </summary>
[Index(nameof(UserName), IsUnique = true)]
public class User : EntityBase
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
    public required string PasswordHash { get; set; }

    [MaxLength(100)]
    public required string Salt { get; set; }

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
