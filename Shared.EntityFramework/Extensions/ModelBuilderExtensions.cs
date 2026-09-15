using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.Entities;

namespace Shared.EntityFramework.Extensions
{
    public static class ModelBuilderExtensions
    {


        public static void ApplySoftDeleteFilter(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType)) continue;
                
                var lambda = GetFilterExpression(entityType.ClrType);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
            
        }

        private static LambdaExpression GetFilterExpression(Type clrType)
        {
            // p => p.DeletedAt == null
            ParameterExpression parameter = Expression.Parameter(clrType,"p");

            MemberExpression propertyAccess = Expression.Property(parameter,nameof(ISoftDeletable.DeletedAt));

            ConstantExpression nullConstant = Expression.Constant(null,typeof(DateTimeOffset?));

            BinaryExpression comparison = Expression.Equal(propertyAccess,nullConstant);

            LambdaExpression lambda = Expression.Lambda(comparison,parameter);

            return lambda;
        }
    }
}