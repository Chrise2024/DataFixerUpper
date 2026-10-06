using System.Runtime.CompilerServices;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// Provide static usage for <see cref="T:DataFixerUpper.Datafixers.Kinds.Kind`2"/>
/// </summary>
public static class Kind
{
    /// <summary>
    /// The witness type base of this <see cref="T:DataFixerUpper.Datafixers.Kinds.Kind`2"/> functor.
    /// </summary>
    public abstract class Mu : Anchor;
}

/// <summary>
/// A type class for a unary type constructor.
/// </summary>
/// <remarks>
/// A type class may be thought of as an interface for types. All types that implement this type class must define the operations specified within that type class. This interface, being the base type for type classes, specifies no required operations.
/// </remarks>
/// <typeparam name="TFunctor">The witness type for the type constructor this type class is defined for.</typeparam>
/// <typeparam name="TMu">The witness type for this type class.</typeparam>
/// <seealso cref="T:DataFixerUpper.Datafixers.Kinds.Kind`2"/>
public partial interface IKind<TFunctor, TMu> : IApp<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Kind.Mu;

/// <summary>
/// Basic implementation of <see cref="T:DataFixerUpper.Datafixers.Kinds.IKind`2"/>.
/// </summary>
/// <remarks>
/// A type class may be thought of as an interface for types. All types that implement this type class must define the operations specified within that type class. This interface, being the base type for type classes, specifies no required operations.
/// </remarks>
/// <typeparam name="TFunctor">The witness type for the type constructor this type class is defined for.</typeparam>
/// <typeparam name="TMu">The witness type for this type class.</typeparam>
/// <seealso cref="T:DataFixerUpper.Datafixers.Kinds.IKind`2"/>
public abstract partial class Kind<TFunctor, TMu> : IKind<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Kind.Mu
{
    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <returns>Unboxed container.</returns>
    public static IKind<TFunctor, TProof> Unbox<TProof>(IApp<TFunctor, TProof> value)
        where TProof : Kind.Mu
    {
        return Unsafe.As<IKind<TFunctor, TProof>>(value);
    }
}