using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Core.Domain.VOs;

namespace Shared.EntityFramework.Converters;

public sealed class BaseIdConverter<TId, TKey>()
    : ValueConverter<TId, TKey>(id => id.Value, CreateFromProviderExpression())
    where TId : BaseId<TKey>
    where TKey : notnull
{
    private static Expression<Func<TKey, TId>> CreateFromProviderExpression()
    {
        var param = Expression.Parameter(typeof(TKey), "value");

        var fromMethod = typeof(TId).GetMethod("From", [typeof(TKey)]);

        if (fromMethod != null)
        {
            return Expression.Lambda<Func<TKey, TId>>(
                Expression.Call(fromMethod, param), param
            );
        }

        // 2. Yoksa public constructor TId(TKey)'i çağır
        var ctor = typeof(TId).GetConstructor([typeof(TKey)])
                   ?? throw new InvalidOperationException(
                       $"{typeof(TId).Name} requires a constructor taking {typeof(TKey).Name} or a static 'From' method.");

        return Expression.Lambda<Func<TKey, TId>>(
            Expression.New(ctor, param), param
        );
    }
}