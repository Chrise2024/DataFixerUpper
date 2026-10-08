// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0

[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
{
    public bool ReturnValue => returnValue;
}

#endif