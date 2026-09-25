using System;
using DataFixerUpper.Extensions;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// Provide static usage for <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/>
/// </summary>
public static class Applicative
{
    /// <summary>
    /// The witness type base of this applicative functor.
    /// </summary>
    public abstract class Mu : Functor.Mu;
}

/// <summary>
/// The <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> with methods for applying <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}})">wrapped transformations</see> and <see cref="M:Point">wrapping values in containers</see>.
/// </summary>
/// <typeparam name="TFunctor">The container type.</typeparam>
/// <typeparam name="TMu">The witness type of this applicative functor.</typeparam>
public abstract partial class Applicative<TFunctor, TMu> : Functor<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Applicative.Mu
{
    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <returns>Unboxed container.</returns>
    public new static Applicative<TFunctor, TMu> Unbox(IApp<TMu, TFunctor> proofBox)
    {
        return (Applicative<TFunctor, TMu>) proofBox;
    }

    /// <summary>
    /// Create container from value.
    /// </summary>
    /// <returns>Container instance.</returns>
    public abstract IApp<TFunctor, T> Point<T>(T instance);

    /// <summary>
    /// Projects contents of arg into a new form with a wrapped transformation function.
    /// </summary>
    /// <param name="selector">The wrapped transformation function.</param>
    /// <param name="app">The input container that will be transformed.</param>
    /// <typeparam name="TSource">The type of the <paramref name="app"/>.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <returns>The transformed container.</returns>
    public virtual IApp<TFunctor, TResult> Select<TSource, TResult>(IApp<TFunctor, Func<TSource, TResult>> selector, IApp<TFunctor, TSource> app)
    {
        return Lift(selector).Apply(app);
    }
}