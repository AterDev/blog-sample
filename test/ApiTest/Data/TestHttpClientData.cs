using Share.Models.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TUnit.Core.Interfaces;
using UserMod.Models.UserDtos;

namespace ApiTest.Data;

public class TestHttpClientData : IAsyncInitializer, IAsyncDisposable
{
    public HttpClient HttpClient { get; private set; } = new();

    public async Task InitializeAsync()
    {
        HttpClient = (GlobalHooks.App ?? throw new NullReferenceException())
            .CreateHttpClient("AdminService");

        if (GlobalHooks.NotificationService != null)
        {
            await GlobalHooks.NotificationService
                .WaitForResourceAsync("AdminService", KnownResourceStates.Running)
                .WaitAsync(TimeSpan.FromSeconds(10));
        }
        await Task.Delay(3000); // Allow some time for the service to be fully ready

        // Authenticate once and set bearer token for subsequent requests
        var loginDto = new LoginDto
        {
            UserName = "admin@default.com",
            Password = "Perigon.2026",
        };

        using var resp = await HttpClient.PostAsJsonAsync("/api/user/login", loginDto);
        var content = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to login for tests. Status: {resp.StatusCode}, Content: {content}");
        }

        var token = await resp.Content.ReadFromJsonAsync<AccessTokenDto>();
        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Failed to acquire access token for tests.");
        }

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }

    public async ValueTask DisposeAsync()
    {
        await Console.Out.WriteLineAsync("Cleaning up HttpClient resources after tests.");
        HttpClient.Dispose();
    }
}
