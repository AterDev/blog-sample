using Entity.UserMod;
namespace UserMod.Models.UserDtos;
/// <summary>
/// 用户FilterDto
/// </summary>
/// <see cref="Entity.UserMod.User"/>
public class UserFilterDto : FilterBase
{
    /// <summary>
    /// 用户名
    /// </summary>
    [MaxLength(50)]
    public string? UserName { get; set; }

}
