using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.System;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var auditEntries = new List<AuditLog>();
            var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

            foreach (var entry in entries)
            {
                // Bỏ qua chính bảng AuditLog để tránh vòng lặp vô tận
                if (entry.Entity is AuditLog || entry.Entity is NotificationLog || entry.Entity is PaymentGatewayLog) continue;

                var auditEntry = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    CreatedAt = DateTimeOffset.UtcNow
                };

                var oldValues = new Dictionary<string, object?>();
                var newValues = new Dictionary<string, object?>();

                foreach (var prop in entry.Properties)
                {
                    if (prop.IsTemporary) continue;
                    
                    if (entry.State == EntityState.Deleted)
                    {
                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        if (prop.IsModified)
                        {
                            oldValues[prop.Metadata.Name] = prop.OriginalValue;
                            newValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        newValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                if (oldValues.Count > 0) auditEntry.OldValues = JsonSerializer.Serialize(oldValues);
                if (newValues.Count > 0) auditEntry.NewValues = JsonSerializer.Serialize(newValues);

                auditEntries.Add(auditEntry);
            }

            if (auditEntries.Count > 0)
            {
                context.Set<AuditLog>().AddRange(auditEntries);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
