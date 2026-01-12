using Microsoft.Extensions.Hosting;
namespace UserMod;

public static class ModuleExtensions
{
    /// <summary>
    /// module services or init task
    /// </summary>
    public static IHostApplicationBuilder AddUserMod(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}