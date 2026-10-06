using System;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// A marker interface representing an applied unary type constructor.
/// </summary>
/// <typeparam name="TAnchor">The type witness representing the type constructor. This is often a nested Mu empty class.</typeparam>
/// <typeparam name="T">The type applied to the type constructor.</typeparam>
public interface IApp<TAnchor, out T>
    where TAnchor : Anchor;

// App2
internal interface IBiApp<TAnchor, T1, T2>
    where TAnchor : BiAnchor;