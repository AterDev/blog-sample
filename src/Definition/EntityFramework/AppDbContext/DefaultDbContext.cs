


namespace EntityFramework.AppDbContext;

/// <summary>
/// default data access for main business
/// </summary>
/// <param name="options"></param>
public partial class DefaultDbContext(DbContextOptions<DefaultDbContext> options)
    : ContextBase(options)
{
    #region CMSMod
    
    
    #endregion

    #region SystemMod
    
    
    
    

    /// <summary>
    /// 菜单
    /// </summary>
    
    

    /// <summary>
    /// 权限组
    /// </summary>
    
    
    
    #endregion


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<SystemLogs>().Ignore(e => e.IsDeleted);

        builder
            .Entity<SystemUser>()
            .HasMany(u => u.SystemRoles)
            .WithMany(r => r.Users)
            .UsingEntity<SystemUserRole>(
                j => j.HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId),
                j => j.HasOne(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId),
                j =>
                {
                    j.HasKey(ur => ur.Id);
                }
            );
        builder
            .Entity<SystemMenu>()
            .HasMany(u => u.SystemRoles)
            .WithMany(r => r.SystemMenus)
            .UsingEntity<SystemMenuRole>(
                j => j.HasOne(ur => ur.SystemRole).WithMany().HasForeignKey(ur => ur.RoleId),
                j => j.HasOne(ur => ur.SystemMenu).WithMany().HasForeignKey(ur => ur.MenuId),
                j =>
                {
                    j.HasKey(ur => ur.Id);
                }
            );
    }
}
