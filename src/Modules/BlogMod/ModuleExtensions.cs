using Microsoft.Extensions.Hosting;
namespace BlogMod;

public static class ModuleExtensions
{
    /// <summary>
    /// module services or init task
    /// </summary>
    public static IHostApplicationBuilder AddBlogMod(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}