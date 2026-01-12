using UserMod.Models.UserDtos;
namespace AdminService.Controllers.UserMod;

/// <summary>
/// 用户
/// </summary>
public class UserController(
    Localizer localizer,
    IUserContext user,
    ILogger<UserController> logger,
    UserManager manager
    ) : RestControllerBase<UserManager>(localizer, manager, user, logger)
{
    /// <summary>
    /// Update 用户 ✍️
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<bool> UpdateAsync([FromRoute] Guid id, UserUpdateDto dto)
    {
        return await _manager.EditAsync(id, dto) == 1;
    }

    /// <summary>
    /// Get 用户 Detail ✍️
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<UserDetailDto?> DetailAsync([FromRoute] Guid id)
    {
        return await _manager.GetAsync(id);
    }
}