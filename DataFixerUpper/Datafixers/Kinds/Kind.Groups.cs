namespace DataFixerUpper.Datafixers.Kinds;

public abstract partial class Kind<TFunctor, TMu>
{
    /// <summary>
    /// Aggregates a value into a product.
    /// </summary>
    /// <param name="t1">Value to wrap.</param>
    /// <returns>Aggregates production.</returns>
    public virtual Products.P1<TFunctor, T1> Group<T1>(
        IApp<TFunctor, T1> t1
    )
    {
        return new Products.P1<TFunctor, T1>(t1);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P2<TFunctor, T1, T2> Group<T1, T2>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2
    )
    {
        return new Products.P2<TFunctor, T1, T2>(t1, t2);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P3<TFunctor, T1, T2, T3> Group<T1, T2, T3>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3
    )
    {
        return new Products.P3<TFunctor, T1, T2, T3>(t1, t2, t3);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P4<TFunctor, T1, T2, T3, T4> Group<T1, T2, T3, T4>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4
    )
    {
        return new Products.P4<TFunctor, T1, T2, T3, T4>(t1, t2, t3, t4);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P5<TFunctor, T1, T2, T3, T4, T5> Group<T1, T2, T3, T4, T5>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5
    )
    {
        return new Products.P5<TFunctor, T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P6<TFunctor, T1, T2, T3, T4, T5, T6> Group<T1, T2, T3, T4, T5, T6>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6
    )
    {
        return new Products.P6<TFunctor, T1, T2, T3, T4, T5, T6>(t1, t2, t3, t4, t5, t6);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> Group<T1, T2, T3, T4, T5, T6, T7>(
        IApp<TFunctor, T1> t1,
        IApp<TFunctor, T2> t2,
        IApp<TFunctor, T3> t3,
        IApp<TFunctor, T4> t4,
        IApp<TFunctor, T5> t5,
        IApp<TFunctor, T6> t6,
        IApp<TFunctor, T7> t7
    )
    {
        return new Products.P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, t4, t5, t6, t7);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> Group<T1, T2, T3, T4, T5, T6, T7, T8>(
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
        return new Products.P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, t4, t5, t6, t7, t8);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
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
        return new Products.P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(t1, t2, t3, t4, t5, t6, t7, t8, t9);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
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
        return new Products.P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
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
        return new Products.P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
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
        return new Products.P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
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
        return new Products.P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
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
        return new Products.P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
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
        return new Products.P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
    }

    /// <summary>
    /// Aggregates values into a product.
    /// </summary>
    /// <remarks>See: <see cref="M:DataFixerUpper.Datafixers.Kinds.Kind`2.Group``1(DataFixerUpper.Datafixers.Kinds.IApp{`0,``0})"/>.</remarks>
    public virtual Products.P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Group<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
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
        return new Products.P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
    }
}