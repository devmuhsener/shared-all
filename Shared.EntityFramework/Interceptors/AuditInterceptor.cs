using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Core.Domain.Entities;

namespace Shared.EntityFramework.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {

            UpdateAuditProperties(eventData.Context);

            return base.SavingChanges(eventData, result);

        }


        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
 )
        {

            UpdateAuditProperties(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        }

        private  void UpdateAuditProperties(DbContext? context)
        {
            if (context == null) return;
            var entries = context.ChangeTracker.Entries<IAuditable>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.MarkAsUpdated();
                }
            }
        }
    }
}