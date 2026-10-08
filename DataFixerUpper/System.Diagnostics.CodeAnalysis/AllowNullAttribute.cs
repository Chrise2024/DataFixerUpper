// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
internal sealed class AllowNullAttribute : Attribute;

#endif