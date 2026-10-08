// ReSharper disable once CheckNamespace

namespace System.Diagnostics.CodeAnalysis;

#if NETSTANDARD2_0

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullAttribute : Attribute
{
    public MemberNotNullAttribute(string member)
    {
        Members = [member];
    }

    public MemberNotNullAttribute(params string[] members)
    {
        Members = members;
    }

    public string[] Members { get; }
}

#endif