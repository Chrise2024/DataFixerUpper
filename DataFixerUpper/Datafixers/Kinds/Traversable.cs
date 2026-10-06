using System;
using System.Runtime.CompilerServices;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// Provide static usage for <see cref="T:DataFixerUpper.Datafixers.Kinds.Traversable`2"/>
/// </summary>
public static class Traversable
{
    /// <summary>
    /// The witness type base of this traversable functor.
    /// </summary>
    public abstract class Mu : Functor.Mu;
}

/// <summary>
/// The traversable type class for some container type takes in an effectful transformation and produces an equivalent effectful transformation on the container type.
/// </summary>
/// <typeparam name="TFunctor">The container type.</typeparam>
/// <typeparam name="TMu">The witness type of this applicative functor.</typeparam>
public interface ITraversable<TFunctor, TMu> : IFunctor<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Traversable.Mu
{
    /// <summary>
    /// Applies a function that produces an <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> effect to each value contained within <paramref name="input"/>, then builds a container with an equivalent structure containing the (unboxed) results.
    /// </summary>
    /// <param name="applicative">An instance of the <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> type class which defines the behavior of <typeparamref name="TFunctor1"/>.</param>
    /// <param name="selector">The function to apply.</param>
    /// <param name="input">The input container.</param>
    /// <typeparam name="TFunctor1">The container type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/>.</typeparam>
    /// <typeparam name="TMu1">The witness type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> functor.</typeparam>
    /// <typeparam name="TSource">The input type.</typeparam>
    /// <typeparam name="TResult">The output type.</typeparam>
    /// <returns>A container holding the results of applying function to each element, if the function was successful for every element.</returns>
    IApp<TFunctor1, IApp<TFunctor, TResult>> Traverse<TFunctor1, TMu1, TSource, TResult>(
        Applicative<TFunctor1, TMu1> applicative,
        Func<TSource, IApp<TFunctor1, TResult>> selector,
        IApp<TFunctor, TSource> input
    )
        where TFunctor1 : Anchor
        where TMu1 : Applicative.Mu;

    /// <summary>
    /// Swaps the order of this container with a nested container in input.
    /// </summary>
    /// <param name="applicative">An instance of the <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> type class which defines the behavior of <typeparamref name="TFunctor1"/>.</param>
    /// <param name="input">The input container.</param>
    /// <typeparam name="TFunctor1">The container type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/>.</typeparam>
    /// <typeparam name="TMu1">The witness type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> functor.</typeparam>
    /// <typeparam name="T">The contained type.</typeparam>
    /// <returns>The nested contained value with the containers swapped.</returns>
    IApp<TFunctor1, IApp<TFunctor, T>> Flip<TFunctor1, TMu1, T>(
        Applicative<TFunctor1, TMu1> applicative,
        IApp<TFunctor, IApp<TFunctor1, T>> input
    )
        where TFunctor1 : Anchor
        where TMu1 : Applicative.Mu;
}

/// <summary>
/// Basic implementation of <see cref="T:DataFixerUpper.Datafixers.Kinds.ITraversable`2"/>.
/// </summary>
/// <typeparam name="TFunctor">The container type.</typeparam>
/// <typeparam name="TMu">The witness type of this applicative functor.</typeparam>
public abstract class Traversable<TFunctor, TMu> : Functor<TFunctor, TMu>, ITraversable<TFunctor, TMu>
    where TFunctor : Anchor
    where TMu : Traversable.Mu
{
    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <returns>Unboxed container.</returns>
    public static Traversable<TFunctor, TMu> Unbox(IApp<TMu, TFunctor> proofBox)
    {
        return Unsafe.As<Traversable<TFunctor, TMu>>(proofBox);
        // return (Traversable<TFunctor, TMu>) proofBox;
    }

    /// <inheritdoc/>
    public abstract IApp<TFunctor1, IApp<TFunctor, TResult>> Traverse<TFunctor1, TMu1, TSource, TResult>(
        Applicative<TFunctor1, TMu1> applicative,
        Func<TSource, IApp<TFunctor1, TResult>> selector,
        IApp<TFunctor, TSource> input
    )
        where TFunctor1 : Anchor
        where TMu1 : Applicative.Mu;

    /// <inheritdoc/>
    public virtual IApp<TFunctor1, IApp<TFunctor, T>> Flip<TFunctor1, TMu1, T>(
        Applicative<TFunctor1, TMu1> applicative,
        IApp<TFunctor, IApp<TFunctor1, T>> input
    )
        where TFunctor1 : Anchor
        where TMu1 : Applicative.Mu
    {
        return Traverse(applicative, Functions.Identity, input);
    }
}