﻿
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.AppDatabase { }

public class AuditIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public AuditIdentityDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Audit> AuditLogs { get; set; }

    public virtual async Task<int> SaveChangesAsync(string userId = null)
    {
        OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        return result;
    }

    public virtual int SaveChanges(string userId = null)
    {
        OnBeforeSaveChanges(userId);
        var result = base.SaveChanges();
        return result;
    }

    public virtual async Task<int> CreateAsync(string userId = null)
    {
        OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync();
        return result;
    }

    private void OnBeforeSaveChanges(string userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;
            var auditEntry = new AuditEntry(entry);
            auditEntry.TableName = entry.Entity.GetType().Name;
            auditEntry.UserId = userId;
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditEntry.TypeAudit.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditEntry.TypeAudit.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditEntry.TypeAudit.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries)
        {
            AuditLogs.Add(auditEntry.ToAudit());
        }
    }
}

