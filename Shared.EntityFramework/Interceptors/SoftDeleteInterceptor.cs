using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Core.Domain.Entities;

namespace Shared.EntityFramework.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {


        public override InterceptionResult<int> SavingChanges(
     DbContextEventData eventData,
     InterceptionResult<int> result
 )
        {

            CallDeleteMethod(eventData.Context);

            return base.SavingChanges(eventData, result);

        }

        private static void CallDeleteMethod(DbContext? context)
        {
            if (context == null) return;
            var entries = context.ChangeTracker.Entries<ISoftDeletable>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.Delete();

                }

            }
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
 )
        {

            CallDeleteMethod(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        }

    }
}