using System.Collections.Generic;
using System.Numerics;

namespace Shared.Core.Domain.Exceptions;

public class IllegalDomainException : BaseDomainException
{
    public IllegalDomainException(string message) : base(message)
    {
    }

    public IllegalDomainException(string message, Exception? innerException) : base(message, innerException)
    {
    }


    public static void ThrowIfRequired(string? value, string propertyName)
    {
        ThrowIfNullOrWhiteSpace(value, propertyName);
    }

    public static void ThrowIfNull(object? value, string propertyName)
    {
        if (value is null)
            throw new IllegalDomainException($"{propertyName} is required.");
    }

    public static void ThrowIfNull(object? value, string propertyName, string message)
    {
        if (value is null)
            throw new IllegalDomainException(message);
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string propertyName, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new IllegalDomainException(message ?? $"{propertyName} is required.");
    }

    public static void ThrowIfEmpty(string? value, string propertyName, string? message = null)
    {
        ThrowIfNullOrWhiteSpace(value, propertyName, message ?? $"{propertyName} is empty.");
    }

    public static void ThrowIfEmpty<T>(IEnumerable<T>? values, string propertyName, string? message = null)
    {
        if (values is null || !values.Any())
            throw new IllegalDomainException(message ?? $"{propertyName} is empty.");
    }

    public static void ThrowIfDefault<T>(T value, string propertyName, string? message = null)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new IllegalDomainException(message ?? $"{propertyName} is required.");
    }

    public static void ThrowIfInvalidLength(string? value, string propertyName, int min, int max)
    {
        ThrowIfInvalidLength(value, propertyName, min, max, trim: false);
    }

    public static void ThrowIfInvalidLength(string? value, string propertyName, int min, int max, bool trim)
    {
        if (min < 0)
            throw new ArgumentOutOfRangeException(nameof(min), "Minimum length cannot be negative.");

        if (max < min)
            throw new ArgumentOutOfRangeException(nameof(max), "Maximum length must be greater than or equal to minimum length.");

        if (value is null)
            return;

        var candidate = trim ? value.Trim() : value;
        if (candidate.Length < min || candidate.Length > max)
            throw new IllegalDomainException($"{propertyName} length must be between {min} and {max}.");
    }

    public static void ThrowIfNegative<T>(T value, string propertyName, string? message = null)
        where T : INumber<T>
    {
        if (value < T.Zero)
            throw new IllegalDomainException(message ?? $"{propertyName} cannot be negative.");
    }

    public static void ThrowIfZero<T>(T value, string propertyName, string? message = null)
        where T : INumber<T>
    {
        if (value == T.Zero)
            throw new IllegalDomainException(message ?? $"{propertyName} cannot be zero.");
    }

    public static void ThrowIfOutOfRange<T>(T value, string propertyName, T min, T max, string? message = null)
        where T : INumber<T>
    {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min), "Minimum value cannot be greater than maximum value.");

        if (value < min || value > max)
            throw new IllegalDomainException(message ?? $"{propertyName} must be between {min} and {max}.");
    }
}
