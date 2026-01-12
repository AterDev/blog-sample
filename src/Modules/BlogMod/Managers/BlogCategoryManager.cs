using BlogMod.Models.BlogCategoryDtos;

namespace BlogMod.Managers;
/// <summary>
/// 博客分类
/// </summary>
public class BlogCategoryManager(
    TenantDbFactory dbContextFactory,
    ILogger<BlogCategoryManager> logger,
    IUserContext userContext
) : ManagerBase<DefaultDbContext, BlogCategory>(dbContextFactory, userContext, logger)
{
    /// <summary>
    /// Filter 博客分类 with paging
    /// </summary>
    public async Task<PageList<BlogCategoryItemDto>> FilterAsync(BlogCategoryFilterDto filter)
    {
        Queryable = Queryable.WhereNotNull(filter.Name, q => q.Name.Contains(filter.Name!));
        return await PageListAsync<BlogCategoryFilterDto, BlogCategoryItemDto>(filter);
    }

    /// <summary>
    /// Add 博客分类
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<BlogCategory> AddAsync(BlogCategoryAddDto dto)
    {
        var entity = dto.MapTo<BlogCategory>();
        entity.UserId = _userContext.UserId;
        await InsertAsync(entity);
        return entity;
    }

    /// <summary>
    /// edit 博客分类
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<int> EditAsync(Guid id, BlogCategoryUpdateDto dto)
    {
        if (await HasPermissionAsync(id))
        {
            return await UpdateAsync(id, dto);
        }
        throw new BusinessException(Localizer.NoPermission);
    }


    /// <summary>
    /// Get 博客分类 detail
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BlogCategoryDetailDto?> GetAsync(Guid id)
    {
        if (await HasPermissionAsync(id))
        {
            return await FindAsync<BlogCategoryDetailDto>(q => q.Id == id);
        }
        throw new BusinessException(Localizer.NoPermission);
    }

    /// <summary>
    /// Delete  博客分类
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
            .Where(q => q.Id == id && q.UserId == _userContext.UserId);
        return await query.AnyAsync();
    }

    public async Task<List<Guid>> GetOwnedIdsAsync(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return [];
        }
        var query = _dbSet
            .Where(q => ids.Contains(q.Id) && q.UserId == _userContext.UserId)
            .Select(q => q.Id);
        return await query.ToListAsync();
    }
}