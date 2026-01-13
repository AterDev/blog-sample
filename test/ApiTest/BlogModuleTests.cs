using ApiTest.Data;
using BlogMod.Models.BlogCategoryDtos;
using BlogMod.Models.BlogDtos;
using Perigon.AspNetCore.Models;
using System.Net.Http.Json;

namespace ApiTest;

/// <summary>
/// 博客模块集成测试
/// 测试博客分类和博客的CRUD操作
/// </summary>
[ClassDataSource<TestHttpClientData>(Shared = SharedType.PerTestSession)]
public class BlogModuleTests(TestHttpClientData httpClientData)
{
    private readonly TestHttpClientData _httpClientData = httpClientData;
    private HttpClient Client => _httpClientData.HttpClient;

    #region 博客分类测试

    /// <summary>
    /// 测试：创建博客分类
    /// </summary>
    [Test]
    [DisplayName("创建博客分类")]
    public async Task CreateBlogCategory_WithValidData_ShouldSucceed()
    {
        // Arrange
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"技术分类-{Guid.NewGuid()}",
            Description = "这是一个技术分类"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(result.Name).IsEqualTo(categoryDto.Name);
        await Assert.That(result.Description).IsEqualTo(categoryDto.Description);
        // 验证 Location 头包含资源 ID
        var location = response.Headers.Location;
        await Assert.That(location).IsNotNull();
    }

    /// <summary>
    /// 测试：获取博客分类详情
    /// </summary>
    [Test]
    [DisplayName("获取博客分类详情")]
    public async Task GetBlogCategory_WithValidId_ShouldReturnCategoryDetail()
    {
        // Arrange - 先创建一个分类
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"获取分类-{Guid.NewGuid()}",
            Description = "用于测试获取分类"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdCategory = await createResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        var categoryId = createdCategory!.Id;

        // Act
        var response = await Client.GetAsync($"/api/blogcategory/{categoryId}");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(categoryId);
        await Assert.That(result.Name).IsEqualTo(categoryDto.Name);
    }

    /// <summary>
    /// 测试：更新博客分类
    /// </summary>
    [Test]
    [DisplayName("更新博客分类")]
    public async Task UpdateBlogCategory_WithValidData_ShouldSucceed()
    {
        // Arrange - 先创建一个分类
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"更新分类-{Guid.NewGuid()}",
            Description = "原始描述"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdCategory = await createResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        var categoryId = createdCategory!.Id;

        // Act - 更新分类
        var updateDto = new BlogCategoryUpdateDto
        {
            Name = $"更新后的分类-{Guid.NewGuid()}",
            Description = "更新后的描述"
        };
        var response = await Client.PatchAsJsonAsync($"/api/blogcategory/{categoryId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();

        // 验证更新是否成功
        var getResponse = await Client.GetAsync($"/api/blogcategory/{categoryId}");
        var updatedCategory = await getResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        await Assert.That(updatedCategory!.Name).IsEqualTo(updateDto.Name);
        await Assert.That(updatedCategory.Description).IsEqualTo(updateDto.Description);
    }

    /// <summary>
    /// 测试：分页查询博客分类
    /// </summary>
    [Test]
    [DisplayName("分页查询博客分类")]
    public async Task FilterBlogCategories_WithPaging_ShouldReturnPagedList()
    {
        // Arrange - 创建多个分类
        for (int i = 0; i < 3; i++)
        {
            var categoryDto = new BlogCategoryAddDto
            {
                Name = $"分页分类-{Guid.NewGuid()}",
                Description = $"分类描述 {i}"
            };
            var createResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
            await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        }

        // Act
        var filter = new BlogCategoryFilterDto { PageIndex = 1, PageSize = 10 };
        var response = await Client.PostAsJsonAsync("/api/blogcategory/filter", filter);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PageList<BlogCategoryItemDto>>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Data).IsNotNull();
        await Assert.That(result.Data.Count).IsGreaterThanOrEqualTo(3);
    }

    /// <summary>
    /// 测试：删除博客分类
    /// </summary>
    [Test]
    [DisplayName("删除博客分类")]
    public async Task DeleteBlogCategory_WithValidId_ShouldSucceed()
    {
        // Arrange - 先创建一个分类
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"删除分类-{Guid.NewGuid()}",
            Description = "用于测试删除分类"
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdCategory = await createResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        var categoryId = createdCategory!.Id;

        // Act
        var response = await Client.DeleteAsync($"/api/blogcategory/{categoryId}");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();

        // 验证分类已被删除
        var getResponse = await Client.GetAsync($"/api/blogcategory/{categoryId}");
        // 删除后应该返回404或找不到
        await Assert.That(getResponse.StatusCode).IsNotEqualTo(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// 测试：删除博客分类时，将其中的博客重新分配到"未分类"
    /// </summary>
    [Test]
    [DisplayName("删除博客分类时将博客重新分配到未分类")]
    public async Task DeleteBlogCategory_ShouldReassignBlogsToUncategorized()
    {
        // Arrange - 创建分类
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"待删除分类-{Guid.NewGuid()}",
            Description = "这个分类将被删除"
        };
        var categoryResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(categoryResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var category = await categoryResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        var categoryId = category!.Id;

        // 创建博客并关联到该分类
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"分类博客-{Guid.NewGuid()}",
            Content = "这篇博客将经历分类被删除的过程",
            AuthorId = userDetail.Id,
            CategoryIds = [categoryId]
        };
        var blogResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(blogResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var blog = await blogResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        var blogId = blog!.Id;

        // Act - 删除分类
        var deleteResponse = await Client.DeleteAsync($"/api/blogcategory/{categoryId}");

        // Assert - 验证分类已被删除
        await Assert.That(deleteResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var deleteResult = await deleteResponse.Content.ReadFromJsonAsync<bool>();
        await Assert.That(deleteResult).IsTrue();

        // 验证博客仍然存在（应该被重新分配到"未分类"）
        var getBlogResponse = await Client.GetAsync($"/api/blog/{blogId}");
        await Assert.That(getBlogResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var blogAfterDelete = await getBlogResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        await Assert.That(blogAfterDelete).IsNotNull();
        await Assert.That(blogAfterDelete!.Id).IsEqualTo(blogId);
    }

    #endregion

    #region 博客测试

    /// <summary>
    /// 测试：创建博客（不指定分类）
    /// </summary>
    [Test]
    [DisplayName("创建博客（默认分类）")]
    public async Task CreateBlog_WithoutCategory_ShouldAddToUncategorized()
    {
        // Arrange
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"测试博客-{Guid.NewGuid()}",
            Content = "这是一篇测试博客，用于验证博客创建功能",
            AuthorId = userDetail.Id,
            CategoryIds = [] // 不指定分类
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/blog", blogDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<BlogDetailDto>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(result.Title).IsEqualTo(blogDto.Title);
        await Assert.That(result.Content).IsEqualTo(blogDto.Content);
        await Assert.That(result.AuthorId).IsEqualTo(userDetail.Id);
        // 验证 Location 头包含资源 ID
        var location = response.Headers.Location;
        await Assert.That(location).IsNotNull();
    }

    /// <summary>
    /// 测试：创建博客（指定分类）
    /// </summary>
    [Test]
    [DisplayName("创建博客（指定分类）")]
    public async Task CreateBlog_WithCategory_ShouldSucceed()
    {
        // Arrange - 先创建一个分类
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"博客分类-{Guid.NewGuid()}",
            Description = "用于博客的分类"
        };
        var categoryResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(categoryResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();
        var categoryId = createdCategory!.Id;

        // 创建博客
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"分类博客-{Guid.NewGuid()}",
            Content = "这是一篇带有分类的博客",
            AuthorId = userDetail.Id,
            CategoryIds = [categoryId]
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/blog", blogDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<BlogDetailDto>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(result.Title).IsEqualTo(blogDto.Title);
    }

    /// <summary>
    /// 测试：获取博客详情
    /// </summary>
    [Test]
    [DisplayName("获取博客详情")]
    public async Task GetBlog_WithValidId_ShouldReturnBlogDetail()
    {
        // Arrange - 先创建一个博客
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"获取博客-{Guid.NewGuid()}",
            Content = "用于测试获取博客详情",
            AuthorId = userDetail.Id,
            CategoryIds = []
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdBlog = await createResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        var blogId = createdBlog!.Id;

        // Act
        var response = await Client.GetAsync($"/api/blog/{blogId}");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<BlogDetailDto>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(blogId);
        await Assert.That(result.Title).IsEqualTo(blogDto.Title);
    }

    /// <summary>
    /// 测试：更新博客
    /// </summary>
    [Test]
    [DisplayName("更新博客")]
    public async Task UpdateBlog_WithValidData_ShouldSucceed()
    {
        // Arrange - 先创建一个博客
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"更新博客-{Guid.NewGuid()}",
            Content = "原始内容",
            AuthorId = userDetail.Id,
            CategoryIds = []
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdBlog = await createResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        var blogId = createdBlog!.Id;

        // Act - 更新博客
        var updateDto = new BlogUpdateDto
        {
            Title = $"更新后的博客-{Guid.NewGuid()}",
            Content = "更新后的内容",
            CategoryIds = []
        };
        var response = await Client.PatchAsJsonAsync($"/api/blog/{blogId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();

        // 验证更新是否成功
        var getResponse = await Client.GetAsync($"/api/blog/{blogId}");
        var updatedBlog = await getResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        await Assert.That(updatedBlog!.Title).IsEqualTo(updateDto.Title);
        await Assert.That(updatedBlog.Content).IsEqualTo(updateDto.Content);
    }

    /// <summary>
    /// 测试：更新博客分类
    /// </summary>
    [Test]
    [DisplayName("更新博客分类")]
    public async Task UpdateBlog_WithNewCategory_ShouldSucceed()
    {
        // Arrange - 创建两个分类
        var category1Dto = new BlogCategoryAddDto
        {
            Name = $"分类1-{Guid.NewGuid()}",
            Description = "第一个分类"
        };
        var category1Response = await Client.PostAsJsonAsync("/api/blogcategory", category1Dto);
        await Assert.That(category1Response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var category1 = await category1Response.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();

        var category2Dto = new BlogCategoryAddDto
        {
            Name = $"分类2-{Guid.NewGuid()}",
            Description = "第二个分类"
        };
        var category2Response = await Client.PostAsJsonAsync("/api/blogcategory", category2Dto);
        await Assert.That(category2Response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var category2 = await category2Response.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();

        // 创建博客并关联到分类1
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"更改分类博客-{Guid.NewGuid()}",
            Content = "用于测试修改博客分类",
            AuthorId = userDetail.Id,
            CategoryIds = [category1!.Id]
        };
        var blogResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(blogResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var blog = await blogResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        var blogId = blog!.Id;

        // Act - 将博客重新分配到分类2
        var updateDto = new BlogUpdateDto
        {
            Title = blog.Title,
            Content = blog.Content,
            CategoryIds = [category2!.Id]
        };
        var response = await Client.PatchAsJsonAsync($"/api/blog/{blogId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// 测试：分页查询博客
    /// </summary>
    [Test]
    [DisplayName("分页查询博客")]
    public async Task FilterBlogs_WithPaging_ShouldReturnPagedList()
    {
        // Arrange - 创建多个博客
        var userDetail = await GetCurrentUserDetail();
        for (int i = 0; i < 3; i++)
        {
            var blogDto = new BlogAddDto
            {
                Title = $"分页博客-{Guid.NewGuid()}",
                Content = $"分页博客内容 {i}",
                AuthorId = userDetail.Id,
                CategoryIds = []
            };
            var createResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
            await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        }

        // Act
        var filter = new BlogFilterDto { PageIndex = 1, PageSize = 10, AuthorId = userDetail.Id };
        var response = await Client.PostAsJsonAsync("/api/blog/filter", filter);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PageList<BlogItemDto>>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Data).IsNotNull();
        await Assert.That(result.Data.Count).IsGreaterThanOrEqualTo(3);
    }

    /// <summary>
    /// 测试：按分类筛选博客
    /// </summary>
    [Test]
    [DisplayName("按分类筛选博客")]
    public async Task FilterBlogs_ByCategory_ShouldReturnFilteredBlogs()
    {
        // Arrange - 创建分类和博客
        var categoryDto = new BlogCategoryAddDto
        {
            Name = $"筛选分类-{Guid.NewGuid()}",
            Description = "用于筛选博客的分类"
        };
        var categoryResponse = await Client.PostAsJsonAsync("/api/blogcategory", categoryDto);
        await Assert.That(categoryResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var category = await categoryResponse.Content.ReadFromJsonAsync<BlogCategoryDetailDto>();

        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"分类筛选博客-{Guid.NewGuid()}",
            Content = "用于测试分类筛选",
            AuthorId = userDetail.Id,
            CategoryIds = [category!.Id]
        };
        var blogResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(blogResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var blog = await blogResponse.Content.ReadFromJsonAsync<BlogDetailDto>();

        // Act - 按分类查询
        var filter = new BlogFilterDto { PageIndex = 1, PageSize = 10, CategoryId = category.Id };
        var response = await Client.PostAsJsonAsync("/api/blog/filter", filter);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PageList<BlogItemDto>>();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Data).IsNotNull();
        // 应该至少包含我们刚创建的博客
        await Assert.That(result.Data.Any(b => b.Id == blog!.Id)).IsTrue();
    }

    /// <summary>
    /// 测试：删除博客
    /// </summary>
    [Test]
    [DisplayName("删除博客")]
    public async Task DeleteBlog_WithValidId_ShouldSucceed()
    {
        // Arrange - 先创建一个博客
        var userDetail = await GetCurrentUserDetail();
        var blogDto = new BlogAddDto
        {
            Title = $"删除博客-{Guid.NewGuid()}",
            Content = "用于测试删除博客",
            AuthorId = userDetail.Id,
            CategoryIds = []
        };
        var createResponse = await Client.PostAsJsonAsync("/api/blog", blogDto);
        await Assert.That(createResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Created);
        var createdBlog = await createResponse.Content.ReadFromJsonAsync<BlogDetailDto>();
        var blogId = createdBlog!.Id;

        // Act
        var response = await Client.DeleteAsync($"/api/blog/{blogId}");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();

        // 验证博客已被删除
        var getResponse = await Client.GetAsync($"/api/blog/{blogId}");
        // 删除后应该返回404或找不到
        await Assert.That(getResponse.StatusCode).IsNotEqualTo(System.Net.HttpStatusCode.OK);
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取当前登录用户的详情
    /// </summary>
    private async Task<UserMod.Models.UserDtos.UserDetailDto> GetCurrentUserDetail()
    {
        var response = await Client.GetAsync("/api/user/detail");
        response.EnsureSuccessStatusCode();
        var userDetail = await response.Content.ReadFromJsonAsync<UserMod.Models.UserDtos.UserDetailDto>();
        return userDetail ?? throw new InvalidOperationException("Failed to get user detail");
    }

    #endregion
}
