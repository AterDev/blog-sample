using UserMod.Models.UserDtos;
using Perigon.AspNetCore.Services;
using Share.Models.Auth;
using Perigon.AspNetCore.Constants;

namespace UserMod.Managers;
/// <summary>
/// 用户
/// </summary>
public class UserManager(
    TenantDbFactory dbContextFactory,
    ILogger<UserManager> logger,
    IUserContext userContext,
    JwtService jwtService
) : ManagerBase<DefaultDbContext, User>(dbContextFactory, userContext, logger)
{
    private readonly JwtService _jwtService = jwtService;
    /// <summary>
    /// Filter 用户 with paging
    /// </summary>
    public async Task<PageList<UserItemDto>> FilterAsync(UserFilterDto filter)
    {
        return await PageListAsync<UserFilterDto, UserItemDto>(filter);
    }

    /// <summary>
    /// Add 用户
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<User> AddAsync(UserAddDto dto)
    {
        var entity = dto.MapTo<User>();

        // 生成盐并加密密码
        if (!string.IsNullOrEmpty(dto.Password))
        {
            var salt = HashCrypto.BuildSalt();
            var passwordHash = HashCrypto.GeneratePwd(dto.Password, salt);
            entity.Salt = salt;
            entity.PasswordHash = passwordHash;
        }

        await InsertAsync(entity);
        return entity;
    }

    /// <summary>
    /// edit 用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<int> EditAsync(Guid id, UserUpdateDto dto)
    {
        if (await HasPermissionAsync(id))
        {
            var user = await FindAsync(id);
            if (user == null)
            {
                throw new BusinessException(Localizer.UserNotFound);
            }
            user = user.Merge(dto);
            // 如果修改密码，需要处理密码加密
            if (!string.IsNullOrEmpty(dto.Password))
            {
                // 使用已有的salt重新加密新密码
                var newPasswordHash = HashCrypto.GeneratePwd(dto.Password, user.Salt);
                user.PasswordHash = newPasswordHash;
            }
            return await _dbContext.SaveChangesAsync();
        }
        throw new BusinessException(Localizer.NoPermission);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    public async Task<AccessTokenDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
        if (user == null)
        {
            throw new BusinessException(Localizer.InvalidUserNameOrPassword);
        }

        // 验证密码
        if (!HashCrypto.Validate(loginDto.Password, user.Salt, user.PasswordHash))
        {
            throw new BusinessException(Localizer.InvalidUserNameOrPassword);
        }

        // 生成JWT令牌和刷新令牌
        var accessToken = _jwtService.GetToken(user.Id.ToString(), [WebConst.User]);
        var refreshToken = JwtService.GetRefreshToken();

        return new AccessTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtService.ExpiredSecond,
            RefreshExpiresIn = _jwtService.RefreshExpiredSecond
        };
    }

    /// <summary>
    /// Get 用户 detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserDetailDto?> GetAsync(Guid id)
    {
        if (await HasPermissionAsync(id))
        {
            return await FindAsync<UserDetailDto>(q => q.Id == id);
        }
        throw new BusinessException(Localizer.NoPermission);
    }

    /// <summary>
    /// Delete  用户
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="softDelete"></param>
    /// <returns></returns>
    public async Task<bool?> DeleteAsync(List<Guid> ids, bool softDelete = true)
    {
        if (!ids.Any())
        {
            return false;
        }
        if (ids.Count() == 1)
        {
            Guid id = ids.First();
            if (await HasPermissionAsync(id))
            {
                return await DeleteOrUpdateAsync(ids, !softDelete) > 0;
            }
            throw new BusinessException(Localizer.NoPermission, StatusCodes.Status403Forbidden);
        }
        else
        {
            var ownedIds = await GetOwnedIdsAsync(ids);
            if (ownedIds.Any())
            {
                return await DeleteOrUpdateAsync(ownedIds, !softDelete) > 0;
            }
            throw new BusinessException(Localizer.NoPermission, StatusCodes.Status403Forbidden);
        }
    }

    public override async Task<bool> HasPermissionAsync(Guid id)
    {
        var query = _dbSet
            .Where(q => q.Id == id);
        return await query.AnyAsync();
    }

    public async Task<List<Guid>> GetOwnedIdsAsync(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return [];
        }
        var query = _dbSet
            .Where(q => ids.Contains(q.Id))
            .Select(q => q.Id);
        return await query.ToListAsync();
    }
}