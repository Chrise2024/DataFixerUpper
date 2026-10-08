// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute;

#endif