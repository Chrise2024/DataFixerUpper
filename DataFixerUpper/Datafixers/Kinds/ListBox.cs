using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using DataFixerUpper.Extensions;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Datafixers.Kinds;

/// <summary>
/// Provide static usage for <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>
/// </summary>
public static class ListBox
{
    /// <summary>
    /// The witness type base of <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>.
    /// </summary>
    public abstract class Mu : Anchor;

    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <param name="box">Boxed <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>.</param>
    /// <typeparam name="T">Wrapped type of <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>.</typeparam>
    /// <returns>Unboxed <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>.</returns>
    public static IList<T> Unbox<T>(IApp<Mu, T> box)
    {
        return Unsafe.As<ListBox<T>>(box).Value;
        // return ((ListBox<T>) box).Value;
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/> from list.
    /// </summary>
    /// <param name="value">List to wrap.</param>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/> wraps the list.</returns>
    public static ListBox<T> Create<T>(IList<T> value)
    {
        return new ListBox<T>(value);
    }

    /// <summary>
    /// Applies an operation to each element of input, then returns a container holding a list of the outputs.
    /// </summary>
    /// <param name="applicative">An instance of the <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> type class which defines the behavior of <typeparamref name="TFunctor"/>.</param>
    /// <param name="selector">The function to apply.</param>
    /// <param name="input">The input container.</param>
    /// <typeparam name="TFunctor">The container type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/>.</typeparam>
    /// <typeparam name="TMu">The witness type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> functor.</typeparam>
    /// <typeparam name="TSource">The input type.</typeparam>
    /// <typeparam name="TResult">The output type.</typeparam>
    /// <returns>A container of the output values.</returns>
    /// <seealso cref="M:DataFixerUpper.Datafixers.Kinds.ListBoxOperator.Traverse``4(DataFixerUpper.Datafixers.Kinds.Applicative{``0,``1},System.Func{``2,DataFixerUpper.Datafixers.Kinds.IApp{``0,``3}},DataFixerUpper.Datafixers.Kinds.IApp{DataFixerUpper.Datafixers.Kinds.ListBox.Mu,``2})"/>
    public static IApp<TFunctor, IList<TResult>> Traverse<TFunctor, TMu, TSource, TResult>(
        Applicative<TFunctor, TMu> applicative,
        Func<TSource, IApp<TFunctor, TResult>> selector,
        IList<TSource> input
    )
        where TFunctor : Anchor
        where TMu : Applicative.Mu
    {
        return applicative.Select(Unbox, ListBoxOperator.Instance.Traverse(applicative, selector, Create(input)));
    }

    /// <summary>
    /// Transforms a list of some container to a container of lists.
    /// </summary>
    /// <param name="applicative">An instance of the <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> type class which defines the behavior of <typeparamref name="TFunctor"/>.</param>
    /// <param name="input">The input container.</param>
    /// <typeparam name="TFunctor">The container type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/>.</typeparam>
    /// <typeparam name="TMu">The witness type of <see cref="T:DataFixerUpper.Datafixers.Kinds.Applicative`2"/> functor.</typeparam>
    /// <typeparam name="T">The contained type.</typeparam>
    /// <returns>A container of list.</returns>
    /// <seealso cref="M:DataFixerUpper.Datafixers.Kinds.Traversable`2.Flip``3(DataFixerUpper.Datafixers.Kinds.Applicative{``0,``1},DataFixerUpper.Datafixers.Kinds.IApp{`0,DataFixerUpper.Datafixers.Kinds.IApp{``0,``2}})"/>
    public static IApp<TFunctor, IList<T>> Flip<TFunctor, TMu, T>(
        Applicative<TFunctor, TMu> applicative,
        IList<IApp<TFunctor, T>> input
    )
        where TFunctor : Anchor
        where TMu : Applicative.Mu
    {
        return applicative.Select(Unbox, ListBoxOperator.Instance.Flip(applicative, Create(input)));
    }
}

/// <summary>
/// Wrapper of a <see cref="T:System.Collections.Generic.IList`1"/>
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public sealed class ListBox<T> : IApp<ListBox.Mu, T>
{
    internal readonly IList<T> Value;

    internal ListBox(IList<T> value)
    {
        Value = value;
    }
}

/// <summary>
/// Operator for <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBox`1"/>.
/// </summary>
public sealed class ListBoxOperator : Traversable<ListBox.Mu, ListBoxOperator.Mu>
{
    /// <summary>
    /// Instance of <see cref="T:DataFixerUpper.Datafixers.Kinds.ListBoxOperator"/>.
    /// </summary>
    public static ListBoxOperator Instance => new();

    /// <inheritdoc/>
    public abstract class Mu : Traversable.Mu;

    private ListBoxOperator() { }

    /// <inheritdoc/>
    public override IApp<ListBox.Mu, TResult> Select<TSource, TResult>(Func<TSource, TResult> selector, IApp<ListBox.Mu, TSource> target)
    {
        return ListBox.Create(ListBox.Unbox(target).Select(selector).ToList());
    }

    /// <inheritdoc/>
    public override IApp<TFunctor1, IApp<ListBox.Mu, TResult>> Traverse<TFunctor1, TMu1, TSource, TResult>(Applicative<TFunctor1, TMu1> applicative, Func<TSource, IApp<TFunctor1, TResult>> selector, IApp<ListBox.Mu, TSource> input)
    {
        IList<TSource> list = ListBox.Unbox(input);
        IApp<TFunctor1, ImmutableList<TResult>.Builder> result = applicative.Point(ImmutableList.CreateBuilder<TResult>());
        foreach (TSource element in list)
        {
            IApp<TFunctor1, TResult> fb = selector.Apply(element);
            Func<ImmutableList<TResult>.Builder, TResult, ImmutableList<TResult>.Builder> combiner = Functions.AddToFirst;
            result = applicative.Combine(applicative.Point(combiner), result, fb);
        }

        return applicative.Select(b => ListBox.Create(b.ToImmutable()), result);
    }
}