using System;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// A marker interface representing an applied unary type constructor.
/// </summary>
/// <typeparam name="TAnchor">The type witness representing the type constructor. This is often a nested Mu empty class.</typeparam>
/// <typeparam name="T">The type applied to the type constructor.</typeparam>
public interface IApp<TAnchor, T>
    where TAnchor : Anchor;

[Obsolete("Not implemented")]
internal interface IApp<TAnchor, T1, T2>
    where TAnchor : Anchor;