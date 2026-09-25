using System;
using DataFixerUpper.Datafixers.Kinds;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs.Impl;

namespace DataFixerUpper.Serialization.Codecs.Builder;

/// <summary>
/// Static usage for <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.
/// </summary>
/// <example>
/// For class: 
/// <code>
/// public class SomeRecord {
///     private int _intValue;
///     private string _stringValue;
///     private List&lt;string&gt; _stringValues;
/// 
///     public SomeRecord(int i, string s, List&lt;string&gt; ls) {
///         _intValue = i;
///         _stringValue = s;
///         _stringValues = ls;
///     }
/// 
///     // methods elided
/// }
/// </code>
/// A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for this type may be created like so:
/// <code>
/// Codec&lt;SomeRecord&gt; codec = RecordCodecBuilder.CreateCodec&lt;SomeRecord&gt;(instance =&gt; instance.Group(
///         Codec.Int.Field("IntValue").ForGetter&lt;SomeRecord&gt;(r =&gt; r.IntValue),
///         Codec.String.Field("StringValue").ForGetter&lt;SomeRecord&gt;(r =&gt; r.StringValue),
///         Codec.String.ImmutableList().Field("StringValues").ForGetter&lt;SomeRecord&gt;(r =&gt; r.StringValues)
///     ).Apply(instance, (i, s, sl) =&gt; new SomeRecord(i, s, sl))
/// );
/// </code>
/// </example>
public static class RecordCodecBuilder
{
    /// <summary>
    /// The witness type base of <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.
    /// </summary>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    // ReSharper disable once UnusedTypeParameter
    public abstract class Mu<TInstance> : Anchor;

    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <param name="box">Boxed <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.</param>
    /// <typeparam name="TInstance">Object type of <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.</typeparam>
    /// <typeparam name="TField">The Field type of <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.</typeparam>
    /// <returns>Unboxed <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.</returns>
    public static RecordCodecBuilder<TInstance, TField> Unbox<TInstance, TField>(IApp<Mu<TInstance>, TField> box)
    {
        return (RecordCodecBuilder<TInstance, TField>) box;
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a single field in some object, given a field name and a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for that field.
    /// </summary>
    /// <param name="getter">Field getter to get field from object.</param>
    /// <param name="codec">A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the fields.</param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a single field in some object.</returns>
    public static RecordCodecBuilder<TInstance, TField> Create<TInstance, TField>(Func<TInstance, TField> getter, MapCodec<TField> codec)
    {
        return new RecordCodecBuilder<TInstance, TField>(getter, _ => codec, codec);
    }

    /// <summary>
    /// Creates <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a single field in some object, given a field name and a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for that field.
    /// </summary>
    /// <param name="getter">Field getter to get field from object.</param>
    /// <param name="fieldName">The name the field is serialized under.</param>
    /// <param name="codec">A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the fields.</param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a single field in some object.</returns>
    public static RecordCodecBuilder<TInstance, TField> Create<TInstance, TField>(Func<TInstance, TField> getter, string fieldName, Codec<TField> codec)
    {
        return Create(getter, codec.Field(fieldName));
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a singleton field value.
    /// The lifecycle of the builder is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="instance"></param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns></returns>
    public static RecordCodecBuilder<TInstance, TField> Point<TInstance, TField>(TField instance)
    {
        return new RecordCodecBuilder<TInstance, TField>(_ => instance, _ => IEncoder<TField>.Empty(), IDecoder<TField>.Unit(instance));
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a singleton field value with given lifecycle.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="lifecycle"></param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns></returns>
    public static RecordCodecBuilder<TInstance, TField> Point<TInstance, TField>(TField instance, Lifecycle lifecycle)
    {
        return new RecordCodecBuilder<TInstance, TField>(_ => instance, _ => IEncoder<TField>.Empty().WithLifecycle(lifecycle), IDecoder<TField>.Unit(instance).WithLifecycle(lifecycle));
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a singleton field value with lifecycle <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/>.
    /// </summary>
    /// <param name="instance"></param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns></returns>
    public static RecordCodecBuilder<TInstance, TField> Stable<TInstance, TField>(TField instance)
    {
        return Point<TInstance, TField>(instance, Lifecycle.Stable);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a singleton field value with lifecycle <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/>.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="since"></param>
    /// <typeparam name="TInstance">The type of the complete object.</typeparam>
    /// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
    /// <returns></returns>
    public static RecordCodecBuilder<TInstance, TField> Deprecated<TInstance, TField>(TField instance, int since)
    {
        return Point<TInstance, TField>(instance, Lifecycle.CreateDeprecated(since));
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> using the fields provided via the given function.
    /// </summary>
    /// <param name="builder">A function that produces a builder for some set of fields.</param>
    /// <typeparam name="T">The type of the complete object.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the complete object type.</returns>
    /// <seealso cref="M:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder.CreateMapCodec``1(System.Func{DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilderOperator{``0},DataFixerUpper.Datafixers.Kinds.IApp{DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder.Mu{``0},``0}})"/>
    public static Codec<T> CreateCodec<T>(
        Func<RecordCodecBuilderOperator<T>, IApp<Mu<T>, T>> builder
    )
    {
        return CreateMapCodec(builder).AsCodec();
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> using the fields provided via the given function.
    /// </summary>
    /// <param name="builder">A function that produces a builder for some set of fields.</param>
    /// <typeparam name="T">The type of the complete object.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the complete object type.</returns>
    /// <seealso cref="M:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder.CreateCodec``1(System.Func{DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilderOperator{``0},DataFixerUpper.Datafixers.Kinds.IApp{DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder.Mu{``0},``0}})"/>
    public static MapCodec<T> CreateMapCodec<T>(
        Func<RecordCodecBuilderOperator<T>, IApp<Mu<T>, T>> builder
    )
    {
        return Build(builder.Apply(RecordCodecBuilderOperator<T>.Instance));
    }

    /// <summary>
    /// Builds a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that operates on the fields defined using the given boxed builder.
    /// </summary>
    /// <param name="builderBox">Boxed <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.</param>
    /// <typeparam name="T">The type of the complete object.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> created from the given builder.</returns>
    public static MapCodec<T> Build<T>(IApp<Mu<T>, T> builderBox)
    {
        RecordCodecBuilder<T, T> builder = Unbox(builderBox);
        return new RecordCodec<T>(builder);
    }
}

/// <summary>
/// A class that enables the aggregation of many <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> into a single codec for some complete type. This allows for building a single codec out of the codecs for many independent fields.
/// </summary>
/// <remarks>Typically, <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> is used to create a codec for a record like the following.</remarks>
/// <param name="getter">Field getter to get field from object.</param>
/// <param name="encoderDispatcher">Dispatch encoder via got field.</param>
/// <param name="decoder">Decoder for this field.</param>
/// <typeparam name="TInstance">The type of the complete object.</typeparam>
/// <typeparam name="TField">The type of the field or collection of fields handled by this builder.</typeparam>
public sealed class RecordCodecBuilder<TInstance, TField>(
    Func<TInstance, TField> getter,
    Func<TInstance, IMapEncoder<TField>> encoderDispatcher,
    IMapDecoder<TField> decoder
)
    : IApp<RecordCodecBuilder.Mu<TInstance>, TField>
{
    internal readonly Func<TInstance, TField> Getter = getter;
    internal readonly Func<TInstance, IMapEncoder<TField>> EncoderDispatcher = encoderDispatcher;
    internal readonly IMapDecoder<TField> Decoder = decoder;

    /// <summary>
    /// Create a dependent <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for another field based on this.
    /// </summary>
    /// <param name="getter">Field getter to get another field from object.</param>
    /// <param name="encoder">Encoder of another field.</param>
    /// <param name="dispatcher">Dispatch another field's decoder form current decoded field.</param>
    /// <typeparam name="TField1">Type of another field.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for another field.</returns>
    public RecordCodecBuilder<TInstance, TField1> Dependent<TField1>(Func<TInstance, TField1> getter, IMapEncoder<TField1> encoder, Func<TField, IMapDecoder<TField1>> dispatcher)
    {
        return new RecordCodecBuilder<TInstance, TField1>(
            getter,
            _ => encoder,
            new DependentRecordDecoder<TField1, TField>(encoder, Decoder, dispatcher)
        );
    }
}

/// <summary>
/// Operator for <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/>.
/// </summary>
/// <typeparam name="TInstance">The type of the complete object.</typeparam>
public sealed class RecordCodecBuilderOperator<TInstance> : Applicative<RecordCodecBuilder.Mu<TInstance>, RecordCodecBuilderOperator<TInstance>.Mu>
{
    /// <inheritdoc/>
    public abstract class Mu : Applicative.Mu;

    /// <summary>
    /// Instance of <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilderOperator`1"/>.
    /// </summary>
    public static RecordCodecBuilderOperator<TInstance> Instance { get; } = new();

    /// <summary>
    /// Unbox boxed <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/>.
    /// </summary>
    /// <param name="box">Boxed value.</param>
    /// <typeparam name="TField">The type of field.</typeparam>
    /// <returns>Unboxed <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/></returns>
    public new RecordCodecBuilder<TInstance, TField> Unbox<TField>(IApp<RecordCodecBuilder.Mu<TInstance>, TField> box)
    {
        return RecordCodecBuilder.Unbox(box);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> for a singleton field value.
    /// The lifecycle of the builder is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="field">Fixed field of builder.</param>
    /// <typeparam name="TField">Field type.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> instance.</returns>
    public override RecordCodecBuilder<TInstance, TField> Point<TField>(TField field)
    {
        return new RecordCodecBuilder<TInstance, TField>(_ => field, _ => IEncoder<TField>.Empty(), IDecoder<TField>.Unit(field));
    }

    /// <summary>
    /// Creates a builder for a singleton field value with given lifecycle.
    /// </summary>
    /// <param name="field">Fixed field of builder.</param>
    /// <param name="lifecycle">Lifecycle of builder.</param>
    /// <typeparam name="TField">Field type.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> instance.</returns>
    public RecordCodecBuilder<TInstance, TField> Point<TField>(TField field, Lifecycle lifecycle)
    {
        return new RecordCodecBuilder<TInstance, TField>(_ => field, _ => IEncoder<TField>.Empty().WithLifecycle(lifecycle), IDecoder<TField>.Unit(field).WithLifecycle(lifecycle));
    }

    /// <summary>
    /// Creates a builder for a singleton field value with lifecycle <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/>.
    /// </summary>
    /// <param name="field">Fixed field of builder.</param>
    /// <typeparam name="TField">Field type.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> instance.</returns>
    public RecordCodecBuilder<TInstance, TField> Stable<TField>(TField field)
    {
        return Point(field, Lifecycle.Stable);
    }

    /// <summary>
    /// Creates a builder for a singleton field value with lifecycle <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/>.
    /// </summary>
    /// <param name="field">Fixed field of builder.</param>
    /// <param name="since">The deprecation version.</param>
    /// <typeparam name="TField">Field type.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> instance.</returns>
    public RecordCodecBuilder<TInstance, TField> Deprecated<TField>(TField field, int since)
    {
        return Point(field, Lifecycle.CreateDeprecated(since));
    }

    
    /// <inheritdoc/>
    public override IApp<RecordCodecBuilder.Mu<TInstance>, T2> Select<T1, T2>(Func<T1, T2> selector, IApp<RecordCodecBuilder.Mu<TInstance>, T1> target)
    {
        RecordCodecBuilder<TInstance, T1> builder = Unbox(target);
        Func<TInstance, T1> getter = builder.Getter;

        return new RecordCodecBuilder<TInstance, T2>(
            getter.Then(selector),
            MappedRecordEncoder<TInstance, T1, T2>.Create(builder),
            builder.Decoder.Map(selector)
        );
    }

    /// <inheritdoc/>
    public override Func<IApp<RecordCodecBuilder.Mu<TInstance>, T1>, IApp<RecordCodecBuilder.Mu<TInstance>, T2>> Lift<T1, T2>(IApp<RecordCodecBuilder.Mu<TInstance>, Func<T1, T2>> function)
    {
        return f1 =>
        {
            RecordCodecBuilder<TInstance, Func<T1, T2>> func = Unbox(function);
            RecordCodecBuilder<TInstance, T1> c1 = Unbox(f1);

            return new RecordCodecBuilder<TInstance, T2>(
                Getter,
                LiftedRecordEncoder<TInstance, T1, T2>.Create(func, c1),
                new LiftedRecordDecoder<TInstance, T1, T2>(func, c1)
            );

            T2 Getter(TInstance i) => func.Getter.Apply(i).Apply(c1.Getter.Apply(i));
        };
    }

    /// <inheritdoc/>
    public override IApp<RecordCodecBuilder.Mu<TInstance>, TR> Combine<T1, T2, TR>(IApp<RecordCodecBuilder.Mu<TInstance>, Func<T1, T2, TR>> combiner, IApp<RecordCodecBuilder.Mu<TInstance>, T1> t1, IApp<RecordCodecBuilder.Mu<TInstance>, T2> t2)
    {
        RecordCodecBuilder<TInstance, Func<T1, T2, TR>> func = Unbox(combiner);
        RecordCodecBuilder<TInstance, T1> f1 = Unbox(t1);
        RecordCodecBuilder<TInstance, T2> f2 = Unbox(t2);

        return new RecordCodecBuilder<TInstance, TR>(
            Getter,
            RecordEncoder2<TInstance, T1, T2, TR>.Create(func, f1, f2),
            new RecordDecoder2<TInstance, T1, T2, TR>(func, f1, f2)
        );

        TR Getter(TInstance i) => func.Getter.Apply(i).Apply(f1.Getter.Apply(i), f2.Getter.Apply(i));
    }

    /// <inheritdoc/>
    public override IApp<RecordCodecBuilder.Mu<TInstance>, TR> Combine<T1, T2, T3, TR>(IApp<RecordCodecBuilder.Mu<TInstance>, Func<T1, T2, T3, TR>> combiner, IApp<RecordCodecBuilder.Mu<TInstance>, T1> t1, IApp<RecordCodecBuilder.Mu<TInstance>, T2> t2, IApp<RecordCodecBuilder.Mu<TInstance>, T3> t3)
    {
        RecordCodecBuilder<TInstance, Func<T1, T2, T3, TR>> func = Unbox(combiner);
        RecordCodecBuilder<TInstance, T1> f1 = Unbox(t1);
        RecordCodecBuilder<TInstance, T2> f2 = Unbox(t2);
        RecordCodecBuilder<TInstance, T3> f3 = Unbox(t3);

        return new RecordCodecBuilder<TInstance, TR>(
            Getter,
            RecordEncoder3<TInstance, T1, T2, T3, TR>.Create(func, f1, f2, f3),
            new RecordDecoder3<TInstance, T1, T2, T3, TR>(func, f1, f2, f3)
        );

        TR Getter(TInstance i) => func.Getter.Apply(i).Apply(f1.Getter.Apply(i), f2.Getter.Apply(i), f3.Getter.Apply(i));
    }

    /// <inheritdoc/>
    public override IApp<RecordCodecBuilder.Mu<TInstance>, TR> Combine<T1, T2, T3, T4, TR>(IApp<RecordCodecBuilder.Mu<TInstance>, Func<T1, T2, T3, T4, TR>> combiner, IApp<RecordCodecBuilder.Mu<TInstance>, T1> t1, IApp<RecordCodecBuilder.Mu<TInstance>, T2> t2, IApp<RecordCodecBuilder.Mu<TInstance>, T3> t3, IApp<RecordCodecBuilder.Mu<TInstance>, T4> t4)
    {
        RecordCodecBuilder<TInstance, Func<T1, T2, T3, T4, TR>> func = Unbox(combiner);
        RecordCodecBuilder<TInstance, T1> f1 = Unbox(t1);
        RecordCodecBuilder<TInstance, T2> f2 = Unbox(t2);
        RecordCodecBuilder<TInstance, T3> f3 = Unbox(t3);
        RecordCodecBuilder<TInstance, T4> f4 = Unbox(t4);

        return new RecordCodecBuilder<TInstance, TR>(
            Getter,
            RecordEncoder4<TInstance, T1, T2, T3, T4, TR>.Create(func, f1, f2, f3, f4),
            new RecordDecoder4<TInstance, T1, T2, T3, T4, TR>(func, f1, f2, f3, f4)
        );

        TR Getter(TInstance i) => func.Getter.Apply(i).Apply(f1.Getter.Apply(i), f2.Getter.Apply(i), f3.Getter.Apply(i), f4.Getter.Apply(i));
    }
}