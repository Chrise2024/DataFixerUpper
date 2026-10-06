using System;

namespace DataFixerUpper.Datafixers.Kinds;

public partial interface IApplicative<TFunctor, TMu>
{
    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <param name="function">The container containing the transformation function.</param>
    /// <returns>The lifted function.</returns>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>> Lift<T1, T2>(IApp<TFunctor, Func<T1, T2>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>> Lift<T1, T2, T3>(IApp<TFunctor, Func<T1, T2, T3>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``3(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1,``2}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>> Lift<T1, T2, T3, T4>(IApp<TFunctor, Func<T1, T2, T3, T4>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``4(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1,``2,``3}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>> Lift<T1, T2, T3, T4, T5>(IApp<TFunctor, Func<T1, T2, T3, T4, T5>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``5(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1,``2,``3,``4}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>> Lift<T1, T2, T3, T4, T5, T6>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``6(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1,``2,``3,``4,``5}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>, IApp<TFunctor, T7>> Lift<T1, T2, T3, T4, T5, T6, T7>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7>> function);

    /// <summary>
    /// Lifts a wrapped transformation function into a function between containers.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Lift``7(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1,``2,``3,``4,``5,``6}})"/>.</remarks>
    Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>, IApp<TFunctor, T7>, IApp<TFunctor, T8>> Lift<T1, T2, T3, T4, T5, T6, T7, T8>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8>> function);
}

public abstract partial class Applicative<TFunctor, TMu>
{
    /// <inheritdoc/>
    public abstract Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>> Lift<T1, T2>(IApp<TFunctor, Func<T1, T2>> function);

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>> Lift<T1, T2, T3>(IApp<TFunctor, Func<T1, T2, T3>> function)
    {
        return (t1, t2) => Combine(function, t1, t2);
    }

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>> Lift<T1, T2, T3, T4>(IApp<TFunctor, Func<T1, T2, T3, T4>> function)
    {
        return (t1, t2, t3) => Combine(function, t1, t2, t3);
    }

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>> Lift<T1, T2, T3, T4, T5>(IApp<TFunctor, Func<T1, T2, T3, T4, T5>> function)
    {
        return (t1, t2, t3, t4) => Combine(function, t1, t2, t3, t4);
    }

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>> Lift<T1, T2, T3, T4, T5, T6>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6>> function)
    {
        return (t1, t2, t3, t4, t5) => Combine(function, t1, t2, t3, t4, t5);
    }

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>, IApp<TFunctor, T7>> Lift<T1, T2, T3, T4, T5, T6, T7>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7>> function)
    {
        return (t1, t2, t3, t4, t5, t6) => Combine(function, t1, t2, t3, t4, t5, t6);
    }

    /// <inheritdoc/>
    public Func<IApp<TFunctor, T1>, IApp<TFunctor, T2>, IApp<TFunctor, T3>, IApp<TFunctor, T4>, IApp<TFunctor, T5>, IApp<TFunctor, T6>, IApp<TFunctor, T7>, IApp<TFunctor, T8>> Lift<T1, T2, T3, T4, T5, T6, T7, T8>(IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8>> function)
    {
        return (t1, t2, t3, t4, t5, t6, t7) => Combine(function, t1, t2, t3, t4, t5, t6, t7);
    }
}