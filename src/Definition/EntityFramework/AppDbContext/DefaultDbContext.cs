using Entity.UserMod;
using Entity.BlogMod;

namespace EntityFramework.AppDbContext;

/// <summary>
/// default data access for main business
/// </summary>
/// <param name="options"></param>
public partial class DefaultDbContext(DbContextOptions<DefaultDbContext> options)
    : ContextBase(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogCategory> BlogCategories { get; set; }
    public DbSet<BlogCategoryRelation> BlogCategoryRelations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 配置博客与作者的关系
        builder.Entity<Blog>()
            .HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // 配置博客与分类的多对多关系（通过中间表）
        builder.Entity<BlogCategoryRelation>()
            .HasOne(bcr => bcr.Blog)
            .WithMany(b => b.BlogCategoryRelations)
            .HasForeignKey(bcr => bcr.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<BlogCategoryRelation>()
            .HasOne(bcr => bcr.Category)
            .WithMany(c => c.BlogCategoryRelations)
            .HasForeignKey(bcr => bcr.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // 配置唯一索引（BlogId + CategoryId）
        builder.Entity<BlogCategoryRelation>()
            .HasIndex(bcr => new { bcr.BlogId, bcr.CategoryId })
            .IsUnique();
    }
}
