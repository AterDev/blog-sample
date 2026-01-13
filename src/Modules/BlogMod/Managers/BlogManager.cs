using BlogMod.Models.BlogDtos;
using BlogMod.Models.BlogCategoryDtos;

namespace BlogMod.Managers;

/// <summary>
/// 博客管理器
/// 负责博客的增删查改，以及创建时的默认分类和按分类筛选
/// </summary>
public class BlogManager(
    TenantDbFactory dbContextFactory,
    ILogger<BlogManager> logger,
    IUserContext userContext
) : ManagerBase<DefaultDbContext, Blog>(dbContextFactory, userContext, logger)
{
    private const string UncategorizedName = "未分类";

    /// <summary>
    /// Filter 博客 with paging
    /// 支持按分类筛选
    /// </summary>
    public async Task<PageList<BlogItemDto>> FilterAsync(BlogFilterDto filter)
    {
        Queryable = Queryable.WhereNotNull(filter.AuthorId, q => q.AuthorId == filter.AuthorId);
        Queryable = Queryable
        .WhereNotNull(filter.CategoryId, q => q.BlogCategoryRelations.Any(r => r.CategoryId == filter.CategoryId));
        return await PageListAsync<BlogFilterDto, BlogItemDto>(filter);
    }

    /// <summary>
    /// Add 博客
    /// 如果未选择分类，则默认归类到"未分类"中
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Blog> AddAsync(BlogAddDto dto)
    {
        var entity = dto.MapTo<Blog>();
        entity.AuthorId = _userContext.UserId;
        await InsertAsync(entity);

        // 获取或创建"未分类"分类
        var uncategorized = await _dbContext.BlogCategories
            .FirstOrDefaultAsync(c => c.Name == UncategorizedName);

        if (uncategorized == null)
        {
            uncategorized = new BlogCategory
            {
                Name = UncategorizedName,
                Description = "未分类的博客"
            };
            _dbContext.BlogCategories.Add(uncategorized);
            await _dbContext.SaveChangesAsync();
        }

        // 如果没有指定分类，则默认添加到"未分类"
        if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
        {
            var relation = new BlogCategoryRelation
            {
                BlogId = entity.Id,
                CategoryId = uncategorized.Id
            };
            _dbContext.BlogCategoryRelations.Add(relation);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            // 添加指定的分类关系
            var relations = dto.CategoryIds.Select(categoryId => new BlogCategoryRelation
            {
                BlogId = entity.Id,
                CategoryId = categoryId
            }).ToList();

            _dbContext.BlogCategoryRelations.AddRange(relations);
            await _dbContext.SaveChangesAsync();
        }

        return entity;
    }

    /// <summary>
    /// edit 博客
    /// 支持修改标题/内容/分类
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<int> EditAsync(Guid id, BlogUpdateDto dto)
    {
        if (await HasPermissionAsync(id))
        {
            var result = await UpdateAsync(id, dto);

            // 如果更新了分类
            if (dto.CategoryIds != null && dto.CategoryIds.Count > 0)
            {
                // 删除现有的分类关系
                var existingRelations = await _dbContext.BlogCategoryRelations
                    .Where(r => r.BlogId == id)
                    .ToListAsync();
                _dbContext.BlogCategoryRelations.RemoveRange(existingRelations);

                // 添加新的分类关系
                var newRelations = dto.CategoryIds.Select(categoryId => new BlogCategoryRelation
                {
                    Id = Guid.CreateVersion7(),
                    BlogId = id,
                    CategoryId = categoryId
                }).ToList();

                _dbContext.BlogCategoryRelations.AddRange(newRelations);
                await _dbContext.SaveChangesAsync();
            }

            return result;
        }
        throw new BusinessException(Localizer.NoPermission);
    }

    /// <summary>
    /// Get 博客 detail
    /// 包含分类信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BlogDetailDto?> GetAsync(Guid id)
    {
        if (await HasPermissionAsync(id))
        {
            var blog = await _dbContext.Blogs
                .Include(b => b.BlogCategories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
            {
                return null;
            }

            var dto = blog.MapTo<BlogDetailDto>();

            // 映射分类信息
            if (blog.BlogCategories != null && blog.BlogCategories.Any())
            {
                dto.Categories = blog.BlogCategories
                    .Select(r => new BlogCategoryItemDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        CreatedTime = r.CreatedTime
                    })
                    .ToList();
            }

            return dto;
        }
        throw new BusinessException(Localizer.NoPermission);
    }

    /// <summary>
    /// Delete 博客
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
                return await ExecuteInTransactionAsync<bool>(async () =>
                {
                    var relations = await _dbContext.BlogCategoryRelations
                                       .Where(r => id == r.BlogId)
                                       .ToListAsync();
                    _dbContext.BlogCategoryRelations.RemoveRange(relations);
                    await _dbContext.SaveChangesAsync();
                    return await DeleteOrUpdateAsync(ids, !softDelete) > 0;
                });
            }
            throw new BusinessException(Localizer.NoPermission, StatusCodes.Status403Forbidden);
        }
        else
        {
            var ownedIds = await GetOwnedIdsAsync(ids);
            if (ownedIds.Any())
            {
                return await ExecuteInTransactionAsync<bool>(async () =>
                {
                    var relations = await _dbContext.BlogCategoryRelations
                        .Where(r => ownedIds.Contains(r.BlogId))
                        .ToListAsync();
                    _dbContext.BlogCategoryRelations.RemoveRange(relations);
                    await _dbContext.SaveChangesAsync();
                    return await DeleteOrUpdateAsync(ownedIds, !softDelete) > 0;
                });
            }
            throw new BusinessException(Localizer.NoPermission, StatusCodes.Status403Forbidden);
        }
    }

    public override async Task<bool> HasPermissionAsync(Guid id)
    {
        var query = _dbSet
            .Where(q => q.Id == id && q.AuthorId == _userContext.UserId);
        return await query.AnyAsync();
    }

    public async Task<List<Guid>> GetOwnedIdsAsync(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return [];
        }
        var query = _dbSet
            .Where(q => ids.Contains(q.Id) && q.AuthorId == _userContext.UserId)
            .Select(q => q.Id);
        return await query.ToListAsync();
    }
}