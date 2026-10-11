using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Domain.VOs;

namespace Shared.EntityFramework.Converters;

public static class StronglyTypedIdConfigurationExtensions
{
    public static void RegisterStronglyTypedIds(this ModelConfigurationBuilder configurationBuilder,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var idTypes = assembly.GetExportedTypes()
                .Where(t => t is { IsAbstract: false, IsGenericTypeDefinition: false } && InheritsFromBaseId(t));

            foreach (var idType in idTypes)
            {
                var keyType = GetBaseIdKeyType(idType);
                if (keyType == null) continue;
                
                var converterType = typeof(BaseIdConverter<,>).MakeGenericType(idType, keyType);

                configurationBuilder.Properties(idType).HaveConversion(converterType);

            }
            
        }
    }

    private static Type? GetBaseIdKeyType(Type idType)
    {
        var current = idType.BaseType;
        while (current != null)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(BaseId<>))
            {
                return current.GetGenericArguments()[0];
            }

            current = current.BaseType;
        }

        return null;
    }

    private static bool InheritsFromBaseId(Type type)
    {
        var current = type.BaseType;
        while (current != null)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(BaseId<>))
                return true;
            current = current.BaseType;
        }

        return false;
    }
}