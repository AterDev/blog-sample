using BlogMod.Models.BlogCategoryDtos;
namespace ApiService.Controllers.BlogMod;

/// <summary>
/// 博客分类
/// </summary>
public class BlogCategoryController(
    Localizer localizer,
    IUserContext user,
    ILogger<BlogCategoryController> logger,
    BlogCategoryManager manager
    ) : RestControllerBase<BlogCategoryManager>(localizer, manager, user, logger)
{
    /// <summary>
    /// list 博客分类 with page ✍️
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [HttpPost("filter")]
    public async Task<ActionResult<PageList<BlogCategoryItemDto>>> ListAsync(BlogCategoryFilterDto filter)
    {
        return await _manager.FilterAsync(filter);
    }

    /// <summary>
    /// Add 博客分类 ✍️
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<BlogCategory>> AddAsync(BlogCategoryAddDto dto)
    {
        
        var entity = await _manager.AddAsync(dto);
        return CreatedAtAction(nameof(DetailAsync), new { id = entity.Id }, entity);
    }

    /// <summary>
    /// Update 博客分类 ✍️
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<bool> UpdateAsync([FromRoute] Guid id, BlogCategoryUpdateDto dto)
    {
        return await _manager.EditAsync(id, dto) == 1;
    }

    /// <summary>
    /// Get 博客分类 Detail ✍️
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<BlogCategoryDetailDto?> DetailAsync([FromRoute] Guid id)
    {
        return await _manager.GetAsync(id);
    }

    /// <summary>
    /// Delete 博客分类 ✍️
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteAsync([FromRoute] Guid id)
    {
        return await _manager.DeleteAsync([id], false);
    }
}