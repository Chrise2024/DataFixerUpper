using System;
using DataFixerUpper.Datafixers.Kinds;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
// Simple container, not need docs

namespace DataFixerUpper.Datafixers;

/// <summary>
/// Product types containing containers.
/// </summary>
public static class Products
{
    public sealed record P1<TFunctor, T1>(
        IApp<TFunctor, T1> Item1
    )
        where TFunctor : Anchor
    {
        public P2<TFunctor, T1, T2> And<T2>(P1<TFunctor, T2> p)
        {
            return new P2<TFunctor, T1, T2>(Item1, p.Item1);
        }

        public P3<TFunctor, T1, T2, T3> And<T2, T3>(P2<TFunctor, T2, T3> p)
        {
            return new P3<TFunctor, T1, T2, T3>(Item1, p.Item1, p.Item2);
        }

        public P4<TFunctor, T1, T2, T3, T4> And<T2, T3, T4>(P3<TFunctor, T2, T3, T4> p)
        {
            return new P4<TFunctor, T1, T2, T3, T4>(Item1, p.Item1, p.Item2, p.Item3);
        }

        public P5<TFunctor, T1, T2, T3, T4, T5> And<T2, T3, T4, T5>(P4<TFunctor, T2, T3, T4, T5> p)
        {
            return new P5<TFunctor, T1, T2, T3, T4, T5>(Item1, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P6<TFunctor, T1, T2, T3, T4, T5, T6> And<T2, T3, T4, T5, T6>(P5<TFunctor, T2, T3, T4, T5, T6> p)
        {
            return new P6<TFunctor, T1, T2, T3, T4, T5, T6>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T2, T3, T4, T5, T6, T7>(P6<TFunctor, T2, T3, T4, T5, T6, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T2, T3, T4, T5, T6, T7, T8>(P7<TFunctor, T2, T3, T4, T5, T6, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T2, T3, T4, T5, T6, T7, T8, T9>(P8<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T2, T3, T4, T5, T6, T7, T8, T9, T10>(P9<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(P10<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(P11<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(P12<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(P13<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(P14<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13, p.Item14);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P15<TFunctor, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13, p.Item14, p.Item15);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1);
        }
    }

    public sealed record P2<TFunctor, T1, T2>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2
    )
        where TFunctor : Anchor
    {
        public P3<TFunctor, T1, T2, T3> And<T3>(P1<TFunctor, T3> p)
        {
            return new P3<TFunctor, T1, T2, T3>(Item1, Item2, p.Item1);
        }

        public P4<TFunctor, T1, T2, T3, T4> And<T3, T4>(P2<TFunctor, T3, T4> p)
        {
            return new P4<TFunctor, T1, T2, T3, T4>(Item1, Item2, p.Item1, p.Item2);
        }

        public P5<TFunctor, T1, T2, T3, T4, T5> And<T3, T4, T5>(P3<TFunctor, T3, T4, T5> p)
        {
            return new P5<TFunctor, T1, T2, T3, T4, T5>(Item1, Item2, p.Item1, p.Item2, p.Item3);
        }

        public P6<TFunctor, T1, T2, T3, T4, T5, T6> And<T3, T4, T5, T6>(P4<TFunctor, T3, T4, T5, T6> p)
        {
            return new P6<TFunctor, T1, T2, T3, T4, T5, T6>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T3, T4, T5, T6, T7>(P5<TFunctor, T3, T4, T5, T6, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T3, T4, T5, T6, T7, T8>(P6<TFunctor, T3, T4, T5, T6, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T3, T4, T5, T6, T7, T8, T9>(P7<TFunctor, T3, T4, T5, T6, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T3, T4, T5, T6, T7, T8, T9, T10>(P8<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T3, T4, T5, T6, T7, T8, T9, T10, T11>(P9<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(P10<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(P11<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(P12<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(P13<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P14<TFunctor, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13, p.Item14);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2);
        }
    }

    public sealed record P3<TFunctor, T1, T2, T3>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3
    )
        where TFunctor : Anchor
    {
        public P4<TFunctor, T1, T2, T3, T4> And<T4>(P1<TFunctor, T4> p)
        {
            return new P4<TFunctor, T1, T2, T3, T4>(Item1, Item2, Item3, p.Item1);
        }

        public P5<TFunctor, T1, T2, T3, T4, T5> And<T4, T5>(P2<TFunctor, T4, T5> p)
        {
            return new P5<TFunctor, T1, T2, T3, T4, T5>(Item1, Item2, Item3, p.Item1, p.Item2);
        }

        public P6<TFunctor, T1, T2, T3, T4, T5, T6> And<T4, T5, T6>(P3<TFunctor, T4, T5, T6> p)
        {
            return new P6<TFunctor, T1, T2, T3, T4, T5, T6>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3);
        }

        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T4, T5, T6, T7>(P4<TFunctor, T4, T5, T6, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T4, T5, T6, T7, T8>(P5<TFunctor, T4, T5, T6, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T4, T5, T6, T7, T8, T9>(P6<TFunctor, T4, T5, T6, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T4, T5, T6, T7, T8, T9, T10>(P7<TFunctor, T4, T5, T6, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T4, T5, T6, T7, T8, T9, T10, T11>(P8<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T4, T5, T6, T7, T8, T9, T10, T11, T12>(P9<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(P10<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(P11<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(P12<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P13<TFunctor, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12, p.Item13);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3);
        }
    }

    public sealed record P4<TFunctor, T1, T2, T3, T4>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4
    )
        where TFunctor : Anchor
    {
        public P5<TFunctor, T1, T2, T3, T4, T5> And<T5>(P1<TFunctor, T5> p)
        {
            return new P5<TFunctor, T1, T2, T3, T4, T5>(Item1, Item2, Item3, Item4, p.Item1);
        }

        public P6<TFunctor, T1, T2, T3, T4, T5, T6> And<T5, T6>(P2<TFunctor, T5, T6> p)
        {
            return new P6<TFunctor, T1, T2, T3, T4, T5, T6>(Item1, Item2, Item3, Item4, p.Item1, p.Item2);
        }

        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T5, T6, T7>(P3<TFunctor, T5, T6, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T5, T6, T7, T8>(P4<TFunctor, T5, T6, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T5, T6, T7, T8, T9>(P5<TFunctor, T5, T6, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T5, T6, T7, T8, T9, T10>(P6<TFunctor, T5, T6, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T5, T6, T7, T8, T9, T10, T11>(P7<TFunctor, T5, T6, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T5, T6, T7, T8, T9, T10, T11, T12>(P8<TFunctor, T5, T6, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T5, T6, T7, T8, T9, T10, T11, T12, T13>(P9<TFunctor, T5, T6, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(P10<TFunctor, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(P11<TFunctor, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P12<TFunctor, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11, p.Item12);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4);
        }
    }

    public sealed record P5<TFunctor, T1, T2, T3, T4, T5>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5
    )
        where TFunctor : Anchor
    {
        public P6<TFunctor, T1, T2, T3, T4, T5, T6> And<T6>(P1<TFunctor, T6> p)
        {
            return new P6<TFunctor, T1, T2, T3, T4, T5, T6>(Item1, Item2, Item3, Item4, Item5, p.Item1);
        }

        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T6, T7>(P2<TFunctor, T6, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T6, T7, T8>(P3<TFunctor, T6, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T6, T7, T8, T9>(P4<TFunctor, T6, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T6, T7, T8, T9, T10>(P5<TFunctor, T6, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T6, T7, T8, T9, T10, T11>(P6<TFunctor, T6, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T6, T7, T8, T9, T10, T11, T12>(P7<TFunctor, T6, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T6, T7, T8, T9, T10, T11, T12, T13>(P8<TFunctor, T6, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T6, T7, T8, T9, T10, T11, T12, T13, T14>(P9<TFunctor, T6, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(P10<TFunctor, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P11<TFunctor, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10, p.Item11);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5);
        }
    }

    public sealed record P6<TFunctor, T1, T2, T3, T4, T5, T6>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6
    )
        where TFunctor : Anchor
    {
        public P7<TFunctor, T1, T2, T3, T4, T5, T6, T7> And<T7>(P1<TFunctor, T7> p)
        {
            return new P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1);
        }

        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T7, T8>(P2<TFunctor, T7, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T7, T8, T9>(P3<TFunctor, T7, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T7, T8, T9, T10>(P4<TFunctor, T7, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T7, T8, T9, T10, T11>(P5<TFunctor, T7, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T7, T8, T9, T10, T11, T12>(P6<TFunctor, T7, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T7, T8, T9, T10, T11, T12, T13>(P7<TFunctor, T7, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T7, T8, T9, T10, T11, T12, T13, T14>(P8<TFunctor, T7, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T7, T8, T9, T10, T11, T12, T13, T14, T15>(P9<TFunctor, T7, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(P10<TFunctor, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9, p.Item10);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6);
        }
    }

    public sealed record P7<TFunctor, T1, T2, T3, T4, T5, T6, T7>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7
    )
        where TFunctor : Anchor
    {
        public P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8> And<T8>(P1<TFunctor, T8> p)
        {
            return new P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1);
        }

        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T8, T9>(P2<TFunctor, T8, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T8, T9, T10>(P3<TFunctor, T8, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T8, T9, T10, T11>(P4<TFunctor, T8, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T8, T9, T10, T11, T12>(P5<TFunctor, T8, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T8, T9, T10, T11, T12, T13>(P6<TFunctor, T8, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T8, T9, T10, T11, T12, T13, T14>(P7<TFunctor, T8, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T8, T9, T10, T11, T12, T13, T14, T15>(P8<TFunctor, T8, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T8, T9, T10, T11, T12, T13, T14, T15, T16>(P9<TFunctor, T8, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8, p.Item9);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7);
        }
    }

    public sealed record P8<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8
    )
        where TFunctor : Anchor
    {
        public P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9> And<T9>(P1<TFunctor, T9> p)
        {
            return new P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1);
        }

        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T9, T10>(P2<TFunctor, T9, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T9, T10, T11>(P3<TFunctor, T9, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T9, T10, T11, T12>(P4<TFunctor, T9, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T9, T10, T11, T12, T13>(P5<TFunctor, T9, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T9, T10, T11, T12, T13, T14>(P6<TFunctor, T9, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T9, T10, T11, T12, T13, T14, T15>(P7<TFunctor, T9, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T9, T10, T11, T12, T13, T14, T15, T16>(P8<TFunctor, T9, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7, p.Item8);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
        }
    }

    public sealed record P9<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9
    )
        where TFunctor : Anchor
    {
        public P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> And<T10>(P1<TFunctor, T10> p)
        {
            return new P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1);
        }

        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T10, T11>(P2<TFunctor, T10, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T10, T11, T12>(P3<TFunctor, T10, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2, p.Item3);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T10, T11, T12, T13>(P4<TFunctor, T10, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T10, T11, T12, T13, T14>(P5<TFunctor, T10, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T10, T11, T12, T13, T14, T15>(P6<TFunctor, T10, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T10, T11, T12, T13, T14, T15, T16>(P7<TFunctor, T10, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6, p.Item7);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9);
        }
    }

    public sealed record P10<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10
    )
        where TFunctor : Anchor
    {
        public P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> And<T11>(P1<TFunctor, T11> p)
        {
            return new P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1);
        }

        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T11, T12>(P2<TFunctor, T11, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1, p.Item2);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T11, T12, T13>(P3<TFunctor, T11, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1, p.Item2, p.Item3);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T11, T12, T13, T14>(P4<TFunctor, T11, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T11, T12, T13, T14, T15>(P5<TFunctor, T11, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T11, T12, T13, T14, T15, T16>(P6<TFunctor, T11, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5, p.Item6);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10);
        }
    }

    public sealed record P11<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11
    )
        where TFunctor : Anchor
    {
        public P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> And<T12>(P1<TFunctor, T12> p)
        {
            return new P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, p.Item1);
        }

        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T12, T13>(P2<TFunctor, T12, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, p.Item1, p.Item2);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T12, T13, T14>(P3<TFunctor, T12, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, p.Item1, p.Item2, p.Item3);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T12, T13, T14, T15>(P4<TFunctor, T12, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T12, T13, T14, T15, T16>(P5<TFunctor, T12, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, p.Item1, p.Item2, p.Item3, p.Item4, p.Item5);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11);
        }
    }

    public sealed record P12<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11,
        IApp<TFunctor, T12> Item12
    )
        where TFunctor : Anchor
    {
        public P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> And<T13>(P1<TFunctor, T13> p)
        {
            return new P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, p.Item1);
        }

        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T13, T14>(P2<TFunctor, T13, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, p.Item1, p.Item2);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T13, T14, T15>(P3<TFunctor, T13, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, p.Item1, p.Item2, p.Item3);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T13, T14, T15, T16>(P4<TFunctor, T13, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, p.Item1, p.Item2, p.Item3, p.Item4);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12);
        }
    }

    public sealed record P13<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11,
        IApp<TFunctor, T12> Item12,
        IApp<TFunctor, T13> Item13
    )
        where TFunctor : Anchor
    {
        public P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> And<T14>(P1<TFunctor, T14> p)
        {
            return new P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, p.Item1);
        }

        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T14, T15>(P2<TFunctor, T14, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, p.Item1, p.Item2);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T14, T15, T16>(P3<TFunctor, T14, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, p.Item1, p.Item2, p.Item3);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13);
        }
    }

    public sealed record P14<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11,
        IApp<TFunctor, T12> Item12,
        IApp<TFunctor, T13> Item13,
        IApp<TFunctor, T14> Item14
    )
        where TFunctor : Anchor
    {
        public P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> And<T15>(P1<TFunctor, T15> p)
        {
            return new P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14, p.Item1);
        }

        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T15, T16>(P2<TFunctor, T15, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14, p.Item1, p.Item2);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14);
        }
    }

    public sealed record P15<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11,
        IApp<TFunctor, T12> Item12,
        IApp<TFunctor, T13> Item13,
        IApp<TFunctor, T14> Item14,
        IApp<TFunctor, T15> Item15
    )
        where TFunctor : Anchor
    {
        public P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> And<T16>(P1<TFunctor, T16> p)
        {
            return new P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14, Item15, p.Item1);
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14, Item15);
        }
    }

    public sealed record P16<TFunctor, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
        IApp<TFunctor, T1> Item1,
        IApp<TFunctor, T2> Item2,
        IApp<TFunctor, T3> Item3,
        IApp<TFunctor, T4> Item4,
        IApp<TFunctor, T5> Item5,
        IApp<TFunctor, T6> Item6,
        IApp<TFunctor, T7> Item7,
        IApp<TFunctor, T8> Item8,
        IApp<TFunctor, T9> Item9,
        IApp<TFunctor, T10> Item10,
        IApp<TFunctor, T11> Item11,
        IApp<TFunctor, T12> Item12,
        IApp<TFunctor, T13> Item13,
        IApp<TFunctor, T14> Item14,
        IApp<TFunctor, T15> Item15,
        IApp<TFunctor, T16> Item16
    )
        where TFunctor : Anchor
    {
        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR> function)
            where TMu : Applicative.Mu
        {
            return Apply(instance, instance.Point(function));
        }

        public IApp<TFunctor, TR> Apply<TR, TMu>(Applicative<TFunctor, TMu> instance, IApp<TFunctor, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR>> function)
            where TMu : Applicative.Mu
        {
            return instance.Combine(function, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11, Item12, Item13, Item14, Item15, Item16);
        }
    }
}