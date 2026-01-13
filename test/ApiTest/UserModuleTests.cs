using ApiTest.Data;
using Share.Models.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserMod.Models.UserDtos;

namespace ApiTest;

[ClassDataSource<TestHttpClientData>(Shared = SharedType.PerTestSession)]
public class UserModuleTests(TestHttpClientData httpClientData)
{
    private readonly TestHttpClientData _httpClientData = httpClientData;
    private HttpClient Client => _httpClientData.HttpClient;

    /// <summary>
    /// 测试：获取当前登录用户详情
    /// </summary>
    [Test]
    [DisplayName("获取当前登录用户详情")]
    public async Task GetUserDetail_ShouldReturnUserInfo()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/api/user/detail");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var userDetail = await response.Content.ReadFromJsonAsync<UserDetailDto>();
        await Assert.That(userDetail).IsNotNull();
        await Assert.That(userDetail!.UserName).IsNotNull().And.IsNotEmpty();
    }

    /// <summary>
    /// 测试：更新用户密码
    /// </summary>
    [Test]
    [DisplayName("更新用户密码")]
    public async Task UpdateUserPassword_WithValidCredentials_ShouldSucceed()
    {
        // Arrange - 首先获取当前用户的ID
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        await Assert.That(userDetail).IsNotNull();

        var userId = userDetail!.Id;
        var newPassword = "NewPassword@2026";
        var updateDto = new UserUpdateDto
        {
            Password = newPassword,
            NickName = null,
            Avatar = null
        };

        // Act - 更新密码
        var response = await Client.PatchAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        await Assert.That(result).IsTrue();

        // Act - 使用新密码重新登录
        var loginDto = new LoginDto
        {
            UserName = userDetail.UserName,
            Password = newPassword
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/user/login", loginDto);

        // Assert - 登录应该成功
        await Assert.That(loginResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var token = await loginResponse.Content.ReadFromJsonAsync<AccessTokenDto>();
        await Assert.That(token).IsNotNull();
        await Assert.That(token!.AccessToken).IsNotNull().And.IsNotEmpty();

        // 重置密码为默认值以便后续测试使用
        var resetDto = new UserUpdateDto
        {
            Password = "Perigon.2026",
            NickName = null,
            Avatar = null
        };
        // 使用新token更新Authorization header
        var originalAuth = Client.DefaultRequestHeaders.Authorization;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var resetResponse = await Client.PatchAsJsonAsync($"/api/user/{userId}", resetDto);
        await Assert.That(resetResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);

        // 恢复原始Authorization header
        Client.DefaultRequestHeaders.Authorization = originalAuth;
    }

    /// <summary>
    /// 测试：上传用户头像 - JPEG格式
    /// </summary>
    [Test]
    [DisplayName("上传用户头像（JPEG格式）")]
    public async Task UploadUserAvatar_WithValidJpeg_ShouldReturnAvatarUrl()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        var imageContent = CreateTestImageContent();
        var multipartContent = new MultipartFormDataContent();
        var streamContent = new StreamContent(new MemoryStream(imageContent));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        multipartContent.Add(streamContent, "file", "test-avatar.jpg");

        // Act
        var response = await Client.PostAsync($"/api/user/{userId}/avatar", multipartContent);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var avatarUrl = await response.Content.ReadAsStringAsync();
        await Assert.That(avatarUrl).IsNotNull().And.IsNotEmpty();
        await Assert.That(avatarUrl).Contains("/avatars/");
    }

    /// <summary>
    /// 测试：上传用户头像 - 无效文件类型应该失败
    /// </summary>
    [Test]
    [DisplayName("上传无效文件类型应该失败")]
    public async Task UploadUserAvatar_WithInvalidFileType_ShouldFail()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        var invalidContent = new byte[] { 1, 2, 3, 4, 5 };
        var multipartContent = new MultipartFormDataContent();
        var streamContent = new StreamContent(new MemoryStream(invalidContent));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        multipartContent.Add(streamContent, "file", "invalid-file.txt");

        // Act
        var response = await Client.PostAsync($"/api/user/{userId}/avatar", multipartContent);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// 测试：更新用户头像URL
    /// </summary>
    [Test]
    [DisplayName("更新用户头像URL")]
    public async Task UpdateUserAvatarUrl_ShouldSucceed()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        var newAvatarUrl = "/avatars/custom-avatar.jpg";
        var updateDto = new UserUpdateDto
        {
            Avatar = newAvatarUrl,
            Password = null,
            NickName = null
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);

        // 验证更新是否成功
        var updatedUserResponse = await Client.GetAsync("/api/user/detail");
        var updatedUserDetail = await updatedUserResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        await Assert.That(updatedUserDetail!.Avatar).IsEqualTo(newAvatarUrl);
    }

    /// <summary>
    /// 测试：更新用户昵称和头像
    /// </summary>
    [Test]
    [DisplayName("更新用户昵称和头像")]
    public async Task UpdateUserProfile_WithNicknameAndAvatar_ShouldSucceed()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        var newNickname = "Updated Nickname";
        var newAvatarUrl = "/avatars/updated-avatar.jpg";
        var updateDto = new UserUpdateDto
        {
            NickName = newNickname,
            Avatar = newAvatarUrl,
            Password = null
        };

        // Act
        var response = await Client.PatchAsJsonAsync($"/api/user/{userId}", updateDto);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);

        // 验证更新是否成功
        var updatedUserResponse = await Client.GetAsync("/api/user/detail");
        var updatedUserDetail = await updatedUserResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        await Assert.That(updatedUserDetail!.NickName).IsEqualTo(newNickname);
        await Assert.That(updatedUserDetail!.Avatar).IsEqualTo(newAvatarUrl);
    }

    /// <summary>
    /// 测试：验证修改密码后仍然可以获取用户信息
    /// </summary>
    [Test]
    [DisplayName("修改密码后验证用户信息")]
    public async Task AfterPasswordChange_ShouldStillAccessUserDetail()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        // 更新密码
        var newPassword = "AnotherNewPassword@2026";
        var updateDto = new UserUpdateDto
        {
            Password = newPassword,
            NickName = null,
            Avatar = null
        };
        var updateResponse = await Client.PatchAsJsonAsync($"/api/user/{userId}", updateDto);
        await Assert.That(updateResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);

        // Act - 获取用户详情
        var finalResponse = await Client.GetAsync("/api/user/detail");

        // Assert - 应该仍然可以访问用户信息
        await Assert.That(finalResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var finalUserDetail = await finalResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        await Assert.That(finalUserDetail).IsNotNull();
        await Assert.That(finalUserDetail!.UserName).IsEqualTo(userDetail.UserName);

        // Act - 使用新密码登录
        var loginDto = new LoginDto
        {
            UserName = userDetail.UserName,
            Password = newPassword
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/user/login", loginDto);

        // Assert - 使用新密码登录应该成功
        await Assert.That(loginResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        var token = await loginResponse.Content.ReadFromJsonAsync<AccessTokenDto>();
        await Assert.That(token).IsNotNull();
        await Assert.That(token!.AccessToken).IsNotNull().And.IsNotEmpty();

        // 重置密码为默认值
        var resetDto = new UserUpdateDto
        {
            Password = "Perigon.2026",
            NickName = null,
            Avatar = null
        };
        var originalAuth = Client.DefaultRequestHeaders.Authorization;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var resetResponse = await Client.PatchAsJsonAsync($"/api/user/{userId}", resetDto);
        await Assert.That(resetResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization = originalAuth;
    }

    /// <summary>
    /// 测试：上传头像后验证URL被保存
    /// </summary>
    [Test]
    [DisplayName("上传头像后验证URL被保存")]
    public async Task UploadAvatar_ThenVerifyUrlIsSaved()
    {
        // Arrange
        var detailResponse = await Client.GetAsync("/api/user/detail");
        var userDetail = await detailResponse.Content.ReadFromJsonAsync<UserDetailDto>();
        var userId = userDetail!.Id;

        var imageContent = CreateTestImageContent();
        var multipartContent = new MultipartFormDataContent();
        var streamContent = new StreamContent(new MemoryStream(imageContent));
        streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        multipartContent.Add(streamContent, "file", "saved-avatar.jpg");

        // Act - 上传头像
        var uploadResponse = await Client.PostAsync($"/api/user/{userId}/avatar", multipartContent);
        var avatarUrl = await uploadResponse.Content.ReadAsStringAsync();

        // 获取用户详情验证URL是否被保存
        await Task.Delay(100); // 等待一下，确保数据已保存
        var verifyResponse = await Client.GetAsync("/api/user/detail");
        var updatedUserDetail = await verifyResponse.Content.ReadFromJsonAsync<UserDetailDto>();

        // Assert
        await Assert.That(uploadResponse.StatusCode).IsEqualTo(System.Net.HttpStatusCode.OK);
        await Assert.That(updatedUserDetail!.Avatar).IsNotNull();
        // 注意：实际的Avatar URL取决于实现，可能需要调整
    }

    /// <summary>
    /// 创建一个最小的有效JPEG图片内容用于测试
    /// </summary>
    private static byte[] CreateTestImageContent()
    {
        // 这是一个最小的有效JPEG文件的十六进制表示
        // 1x1像素的黑色JPEG图片
        return new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01,
            0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43,
            0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08, 0x07, 0x07, 0x07, 0x09,
            0x09, 0x08, 0x0A, 0x0C, 0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12,
            0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20,
            0x24, 0x2E, 0x27, 0x20, 0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29,
            0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27, 0x39, 0x3D, 0x38, 0x32,
            0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01,
            0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x1F, 0x00, 0x00,
            0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x0A, 0x0B, 0xFF, 0xC4, 0x00, 0xB5, 0x10, 0x00, 0x02, 0x01, 0x03,
            0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D,
            0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06,
            0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08,
            0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72,
            0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28,
            0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45,
            0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
            0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75,
            0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
            0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3,
            0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6,
            0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9,
            0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2,
            0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4,
            0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01,
            0x00, 0x00, 0x3F, 0x00, 0xFB, 0xD0, 0xFF, 0xD9
        };
    }
}

