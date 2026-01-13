namespace UserMod;

public class InitModule
{
    /// <summary>
    /// 模块初始化方法
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static async Task InitializeAsync(IServiceProvider provider)
    {
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var context = provider.GetRequiredService<DefaultDbContext>();
        var logger = loggerFactory.CreateLogger<InitModule>();

        try
        {
            var hasUser = await context.Users.AnyAsync();
            if (!hasUser)
            {
                var hashSalt = HashCrypto.BuildSalt();
                var password = HashCrypto.GeneratePwd("Perigon.2026", hashSalt);
                var user = new User
                {
                    UserName = "admin@default.com",
                    PasswordHash = password,
                    NickName = "Admin User",
                    Salt = hashSalt
                };
                context.Add(user);
                await context.SaveChangesAsync();
            }

        }
        catch (Exception ex)
        {
            logger.LogError("Failed to initialize user! {message}. ", ex.Message);
        }
    }


}
