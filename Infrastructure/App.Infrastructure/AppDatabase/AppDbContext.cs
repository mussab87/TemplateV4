using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.AppDatabase { }

public class AppDbContext : AuditIdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.Entity<RootCompanyUsers>().HasKey(sc => new { sc.ApplicationUserId, sc.RootCompanyId });
        //modelBuilder.Entity<RootCompanyForeignAgent>().HasKey(sc => new { sc.RootCompanyId, sc.ForeignAgentId });
        //modelBuilder.Entity<RootCompanyLocalAgent>().HasKey(sc => new { sc.RootCompanyId, sc.LocalAgentId });
        //modelBuilder.Entity<CVAttachment>().HasKey(sc => new { sc.CVId, sc.AttachmentId });
        //modelBuilder.Entity<CVCandidateSkills>().HasKey(sc => new { sc.CVId, sc.CandidateSkillsId });

        //change AspNet Users tables names
        var entityTypes = modelBuilder.Model.GetEntityTypes();
        foreach (var entityType in entityTypes)
            modelBuilder.Entity(entityType.ClrType)
                   .ToTable(entityType.GetTableName().Replace("AspNet", ""));
    }

    public DbSet<UserPassword> UserPassword { get; set; }
    public DbSet<UserTransaction> UserTransaction { get; set; }

    public DbSet<AttachmentType> AttachmentTypes { get; set; }

    public DbSet<Attachment> Attachments { get; set; }
}

