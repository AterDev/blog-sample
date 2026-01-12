using UserMod.Models.UserDtos;

namespace UserMod.Managers;
/// <summary>
/// 用户
/// </summary>
public class UserManager(
    TenantDbFactory dbContextFactory, 
    ILogger<UserManager> logger,
    IUserContext userContext
) : ManagerBase<DefaultDbContext, User>(dbContextFactory, userContext, logger)
{
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
            return await UpdateAsync(id, dto);
        }
        throw new BusinessException(Localizer.NoPermission);
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