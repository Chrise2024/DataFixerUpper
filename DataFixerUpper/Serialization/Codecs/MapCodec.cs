using System;
using DataFixerUpper.Serialization.Codecs.Builder;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Static factory methods for creating <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> instances.
/// </summary>
public static class MapCodec
{
    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> from the given <paramref name="encoder"/> and <paramref name="decoder"/>.
    /// </summary>
    /// <param name="encoder">The <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> used to serialize the fields of the value.</param>
    /// <param name="decoder">The <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> used to parse the fields of the value.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes with <paramref name="encoder"/> and decodes with <paramref name="decoder"/>.</returns>
    public static MapCodec<T> Create<T>(IMapEncoder<T> encoder, IMapDecoder<T> decoder)
    {
        return new SimpleMapCodec<T>(encoder, decoder);
    }

    /// <summary>
    /// Creates a named <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> from the given <paramref name="encoder"/> and <paramref name="decoder"/>.
    /// </summary>
    /// <param name="encoder">The <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> used to serialize the fields of the value.</param>
    /// <param name="decoder">The <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> used to parse the fields of the value.</param>
    /// <param name="codecNameHolder">Holder of the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes with <paramref name="encoder"/>, decodes with <paramref name="decoder"/> and is named by <paramref name="codecNameHolder"/>.</returns>
    public static MapCodec<T> Create<T>(IMapEncoder<T> encoder, IMapDecoder<T> decoder, ValueHolder<string> codecNameHolder)
    {
        return new SimpleMapCodec<T>(encoder, decoder, codecNameHolder);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that writes no fields when encoding and always decodes the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to decode to.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a constant value.</returns>
    public static MapCodec<T> CreateUnit<T>(T value)
    {
        return new UnitMapCodec<T>(value);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that writes no fields when encoding and always decodes the value provided by <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Provider of the value to decode to.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a constant value.</returns>
    /// <remarks>
    /// <paramref name="value"/> is only evaluated when the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is used.
    /// </remarks>
    public static MapCodec<T> CreateUnit<T>(Provider<T> value)
    {
        return new UnitMapCodec<T>(value);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a pair of values.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the left value of the pair.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the right value of the pair.</param>
    /// <typeparam name="TLeft">The type of the left value of the pair.</typeparam>
    /// <typeparam name="TRight">The type of the right value of the pair.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that combines the fields written by <paramref name="left"/> and <paramref name="right"/>.</returns>
    public static MapCodec<(TLeft, TRight)> CreatePair<TLeft, TRight>(MapCodec<TLeft> left, MapCodec<TRight> right)
    {
        return new PairMapCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes with <paramref name="left"/> and falls back to <paramref name="right"/> if the input cannot be decoded by the left codec.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> of the left alternative.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> of the right alternative.</param>
    /// <typeparam name="TLeft">The type of the value handled by <paramref name="left"/>.</typeparam>
    /// <typeparam name="TRight">The type of the value handled by <paramref name="right"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an <see cref="T:DataFixerUpper.Utils.Either`2"/> value, wrapping the first alternative that succeeds.</returns>
    /// <remarks>
    /// Both alternatives read their fields from the same map, and encoding uses the alternative matching the side of the <see cref="T:DataFixerUpper.Utils.Either`2"/> value.
    /// </remarks>
    public static MapCodec<Either<TLeft, TRight>> CreateEither<TLeft, TRight>(MapCodec<TLeft> left, MapCodec<TRight> right)
    {
        return new EitherMapCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes with either <paramref name="left"/> or <paramref name="right"/>, requiring exactly one of them to succeed.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> of the left alternative.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> of the right alternative.</param>
    /// <typeparam name="TLeft">The type of the value handled by <paramref name="left"/>.</typeparam>
    /// <typeparam name="TRight">The type of the value handled by <paramref name="right"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an <see cref="T:DataFixerUpper.Utils.Either`2"/> value of two mutually exclusive alternatives.</returns>
    /// <remarks>
    /// Decoding fails if both <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>s can decode the input, because the correct alternative cannot be determined.
    /// </remarks>
    public static MapCodec<Either<TLeft, TRight>> CreateXor<TLeft, TRight>(MapCodec<TLeft> left, MapCodec<TRight> right)
    {
        return new XorMapCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that handles the given <paramref name="codec"/> as a map.
    /// </summary>
    /// <param name="codec">The codec that encodes to, and decodes from, a map.</param>
    /// <typeparam name="T">The type of the value handled by the codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> backed by <paramref name="codec"/>.</returns>
    /// <remarks>
    /// No check is made that <paramref name="codec"/> actually produces a map, so the caller is responsible for that. On ops that compress maps, the value is written to a single field named <c>value</c>.
    /// </remarks>
    public static MapCodec<T> AssumeMapUnsafe<T>(Codec<T> codec)
    {
        return new CodecMapCodec<T>(codec);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that can refer to itself, allowing recursive data structures to be described.
    /// </summary>
    /// <param name="name">The name of the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>, used for diagnostics.</param>
    /// <param name="wrapped">The function that receives the codec of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> and returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that is actually used for encoding and decoding.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A recursive <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> named by <paramref name="name"/>.</returns>
    /// <remarks>
    /// <paramref name="wrapped"/> is applied lazily, so it may refer to the returned <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> while it is being built.
    /// </remarks>
    public static MapCodec<T> CreateRecursive<T>(string name, Func<Codec<T>, MapCodec<T>> wrapped)
    {
        return new RecursiveMapCodec<T>(name, wrapped);
    }
}

/// <summary>
/// A specialized version of <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that serializes and deserializes a fixed set of record fields.
/// </summary>
/// <remarks>
/// A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is not itself a codec, but it may be turned into one with <see cref="M:DataFixerUpper.Serialization.Codecs.MapCodec`1.AsCodec"/>.
/// </remarks>
/// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>
public abstract partial class MapCodec<T> : CompressorHolder, IMapEncoder<T>, IMapDecoder<T>
{
    /// <summary>
    /// Gets the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.
    /// </summary>
    public abstract ValueHolder<string> CodecNameHolder { get; }

    /// <inheritdoc/>
    public abstract IRecordBuilder<TObject> Encode<TObject>(
        T input,
        DynamicOps<TObject> ops,
        IRecordBuilder<TObject> prefix
    )
        where TObject : notnull;

    
    /// <inheritdoc/>
    public abstract DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
        where TObject : notnull;

    IMapEncoder<T> IMapEncoder<T>.WithLifecycle(Lifecycle lifecycle)
    {
        return WithLifecycle(lifecycle);
    }

    IMapDecoder<T> IMapDecoder<T>.WithLifecycle(Lifecycle lifecycle)
    {
        return WithLifecycle(lifecycle);
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.
    /// </summary>
    /// <returns>This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public IMapEncoder<T> AsMapEncoder()
    {
        return this;
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.
    /// </summary>
    /// <returns>This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public IMapDecoder<T> AsMapDecoder()
    {
        return this;
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> as an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> backed by <see cref="M:DataFixerUpper.Serialization.Codecs.MapCodec`1.AsCodec"/>.</returns>
    public IDecoder<T> AsDecoder()
    {
        return AsCodec();
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> as an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> backed by <see cref="M:DataFixerUpper.Serialization.Codecs.MapCodec`1.AsCodec"/>.</returns>
    public IEncoder<T> AsEncoder()
    {
        return AsCodec();
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> that handles this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> as the field of an instance of <typeparamref name="TInstance"/>.
    /// </summary>
    /// <param name="getter">The function that extracts the field value from an instance of <typeparamref name="TInstance"/>.</param>
    /// <typeparam name="TInstance">The type of the instance that contains the field.</typeparam>
    /// <returns>A builder that can be combined with other builders into a record codec.</returns>
    /// <remarks>
    /// This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is used for both encoding and decoding the field.
    /// </remarks>
    public RecordCodecBuilder<TInstance, T> ForGetter<TInstance>(Func<TInstance, T> getter)

    {
        return new RecordCodecBuilder<TInstance, T>(getter, _ => this, this);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Builder.RecordCodecBuilder`2"/> that handles this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> as the field of an instance of <typeparamref name="TInstance"/>, using a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that depends on the instance.
    /// </summary>
    /// <param name="getter">The function that extracts the field value from an instance of <typeparamref name="TInstance"/>.</param>
    /// <param name="dispatcher">The function that returns the encoder used for an instance of <typeparamref name="TInstance"/>.</param>
    /// <typeparam name="TInstance">The type of the instance that contains the field.</typeparam>
    /// <returns>A builder that can be combined with other builders into a record codec.</returns>
    public RecordCodecBuilder<TInstance, T> ForGetter<TInstance>(Func<TInstance, T> getter, Func<TInstance, IMapEncoder<T>> dispatcher)

    {
        return new RecordCodecBuilder<TInstance, T>(getter, dispatcher, this);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that uses this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> to encode and decode values.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> backed by this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public virtual Codec<T> AsCodec()
    {
        return new MapCodecCodec<T>(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return CodecNameHolder.Value;
    }

    /// <summary>
    /// Returns the name of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> with the given <paramref name="transform"/> applied to it, in the form <c>Name[transform]</c>.
    /// </summary>
    /// <param name="transform">The transformation to apply to the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</param>
    /// <returns>The transformed name of the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public string ToString(string transform)
    {
        return ToString() + "[" + transform + "]";
    }

    /// <summary>
    /// Transforms the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> produced by the encoding and decoding operations of a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.
    /// </summary>
    public interface IResultMapper
    {
        /// <summary>
        /// Transforms the result of a decoding operation.
        /// </summary>
        /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
        /// <param name="input">The map that was decoded.</param>
        /// <param name="original">The result produced by the <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>The transformed result.</returns>
        DataResult<T> Apply<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input, DataResult<T> original)
            where TObject : notnull;

        /// <summary>
        /// Transforms the result of an encoding operation.
        /// </summary>
        /// <param name="ops">The ops used to create the encoded fields.</param>
        /// <param name="input">The value that was encoded.</param>
        /// <param name="original">The record builder produced by the <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>The transformed record builder.</returns>
        IRecordBuilder<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, IRecordBuilder<TObject> original)
            where TObject : notnull;
    }
}
