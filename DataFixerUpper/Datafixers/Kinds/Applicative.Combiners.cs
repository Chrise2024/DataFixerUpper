using System;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Datafixers.Kinds;

#pragma warning disable CS1574

public abstract partial class Applicative<TFunctor, TMu>
{
    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <param name="combiner">Combine function.</param>
    /// <param name="t">Another container.</param>
    /// <returns>Combined Container.</returns>
    public virtual IApp<TFunctor, TR> Combine<T, TR>(
        IApp<TFunctor, Func<T, TR>> combiner,
        IApp<TFunctor, T> t
    )
    {
        return Select(combiner, t);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>
    /// See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.
    /// </remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, TR>(
        IApp<TFunctor, Func<T1, T2, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2
    )
    {
        return Select(Select(Select(Functions.Curry1, combiner), t1), t2);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>
    /// See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.
    /// </remarks>
    /// <returns>Combined Container.</returns>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, TR>(
        IApp<TFunctor, Func<T1, T2, T3, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3
    )
    {
        return Combine(Select(Select(Functions.Curry1, combiner), t1), t2, t3);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4
    )
    {
        return Combine(Combine(Select(Functions.Curry2, combiner), t1, t2), t3, t4);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5
    )
    {
        return Combine(Combine(Select(Functions.Curry2, combiner), t1, t2), t3, t4, t5);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6
    )
    {
        return Combine(Combine(Select(Functions.Curry3, combiner), t1, t2, t3), t4, t5, t6);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7
    )
    {
        return Combine(Combine(Select(Functions.Curry3, combiner), t1, t2, t3), t4, t5, t6, t7);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8
    )
    {
        return Combine(Combine(Select(Functions.Curry4, combiner), t1, t2, t3, t4), t5, t6, t7, t8);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9
    )
    {
        return Combine(Combine(Select(Functions.Curry4, combiner), t1, t2, t3, t4), t5, t6, t7, t8, t9);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10
    )
    {
        return Combine(Combine(Select(Functions.Curry5, combiner), t1, t2, t3, t4, t5), t6, t7, t8, t9, t10);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11
    )
    {
        return Combine(Combine(Select(Functions.Curry5, combiner), t1, t2, t3, t4, t5), t6, t7, t8, t9, t10, t11);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11,
        IApp<TFunctor, T12> t12
    )
    {
        return Combine(Combine(Select(Functions.Curry6, combiner), t1, t2, t3, t4, t5, t6), t7, t8, t9, t10, t11, t12);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11,
        IApp<TFunctor, T12> t12,
        IApp<TFunctor, T13> t13
    )
    {
        return Combine(Combine(Select(Functions.Curry6, combiner), t1, t2, t3, t4, t5, t6), t7, t8, t9, t10, t11, t12, t13);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11,
        IApp<TFunctor, T12> t12,
        IApp<TFunctor, T13> t13,
        IApp<TFunctor, T14> t14
    )
    {
        return Combine(Combine(Select(Functions.Curry7, combiner), t1, t2, t3, t4, t5, t6, t7), t8, t9, t10, t11, t12, t13, t14);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11,
        IApp<TFunctor, T12> t12,
        IApp<TFunctor, T13> t13,
        IApp<TFunctor, T14> t14,
        IApp<TFunctor, T15> t15
    )
    {
        return Combine(Combine(Select(Functions.Curry7, combiner), t1, t2, t3, t4, t5, t6, t7), t8, t9, t10, t11, t12, t13, t14, t15);
    }

    /// <summary>
    /// Combine contents of container with wrapped <see cref="combiner"/>.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Applicative`2.Combine``2(DataFixerUpper.Datafixers.Kinds.IApp{`0,System.Func{``0,``1}},DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual IApp<TFunctor, TR> Combine<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR>(
        IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR>> combiner,
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7,
        IApp<TFunctor, T8> t8,
        IApp<TFunctor, T9> t9,
        IApp<TFunctor, T10> t10,
        IApp<TFunctor, T11> t11,
        IApp<TFunctor, T12> t12,
        IApp<TFunctor, T13> t13,
        IApp<TFunctor, T14> t14,
        IApp<TFunctor, T15> t15,
        IApp<TFunctor, T16> t16
    )
    {
        return Combine(Combine(Select(Functions.Curry8, combiner), t1, t2, t3, t4, t5, t6, t7, t8), t9, t10, t11, t12, t13, t14, t15, t16);
    }
}