using UserMod.Models.UserDtos;
using Share.Models.Auth;
using Share.Exceptions;

namespace AdminService.Controllers.UserMod;

/// <summary>
/// 用户
/// </summary>
public class UserController(
    Localizer localizer,
    IUserContext user,
    ILogger<UserController> logger,
    UserManager manager,
    CommonManager commonManager
    ) : RestControllerBase<UserManager>(localizer, manager, user, logger)
{
    private readonly CommonManager _commonManager = commonManager;

    /// <summary>
    /// 用户登录 ✅
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<AccessTokenDto> LoginAsync([FromBody] LoginDto loginDto)
    {
        return await _manager.LoginAsync(loginDto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<User> AddAsync([FromBody] UserAddDto dto)
    {
        return await _manager.AddAsync(dto);
    }


    /// <summary>
    /// Update 用户 ✅
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [AllowAnonymous]
    public async Task<bool> UpdateAsync([FromRoute] Guid id, UserUpdateDto dto)
    {
        return await _manager.EditAsync(id, dto) == 1;
    }

    /// <summary>
    /// Get 用户 Detail ✅
    /// </summary>
    /// <returns></returns>
    [HttpGet("detail")]
    public async Task<UserDetailDto?> DetailAsync()
    {
        return await _manager.GetAsync(_user.UserId);
    }

    /// <summary>
    /// 上传用户头像 ✅
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="file">头像文件</param>
    /// <returns></returns>
    [HttpPost("{id}/avatar")]
    public async Task<string> UploadAvatarAsync([FromRoute] Guid id, IFormFile file)
    {
        if (file == null)
        {
            throw new BusinessException(Localizer.FileIsEmpty);
        }
        // 允许的图片格式
        var allowedExtensions = new[] { "jpg", "jpeg", "png", "gif", "webp" };
        // 上传文件
        var avatarUrl = await _commonManager.UploadFileAsync(file, "avatars", allowedExtensions);
        return avatarUrl;
    }
}