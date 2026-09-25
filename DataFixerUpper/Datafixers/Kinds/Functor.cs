using System;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// Provide static usage for <see cref="T:DataFixerUpper.Datafixers.Kinds.Functor`2"/>
/// </summary>
public static class Functor
{
    /// <summary>
    /// The witness type base of this <see cref="T:DataFixerUpper.Datafixers.Kinds.Functor`2"/> functor.
    /// </summary>
    public abstract class Mu : Kind.Mu;
}

/// <summary>
/// Defines <see cref="M:DataFixerUpper.Datafixers.Kinds.Functor`2.Select``2(System.Func{``0,``1},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/> method to transform container.
/// </summary>
/// <typeparam name="TFunctor">The container type.</typeparam>
/// <typeparam name="TMu">The witness type of this functor.</typeparam>
public abstract class Functor<TFunctor, TMu> : Kind<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Functor.Mu
{
    
    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> into container.
    /// </summary>
    /// <returns>Unboxed container.</returns>
    public static Functor<TFunctor, TMu> Unbox(IApp<TMu, TFunctor> proofBox)
    {
        return (Functor<TFunctor, TMu>) proofBox;
    }

    /// <summary>
    /// Projects contents of arg into a new form with a wrapped transformation function.
    /// </summary>
    /// <param name="selector">The wrapped transformation function.</param>
    /// <param name="target">The input container that will be transformed.</param>
    /// <typeparam name="TSource">The type of the <paramref name="target"/>.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <returns>The transformed container.</returns>
    public abstract IApp<TFunctor, TResult> Select<TSource, TResult>(Func<TSource, TResult> selector, IApp<TFunctor, TSource> target);
}