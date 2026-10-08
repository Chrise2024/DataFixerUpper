// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
internal sealed class MaybeNullAttribute : Attribute;

#endif