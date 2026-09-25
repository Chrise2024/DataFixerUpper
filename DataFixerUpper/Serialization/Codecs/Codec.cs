using System;
using System.Collections.Generic;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Static factory methods for creating <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> instances.
/// </summary>
public static partial class Codec
{
    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> from the given <paramref name="encoder"/> and <paramref name="decoder"/>.
    /// </summary>
    /// <param name="encoder">The encoder used to serialize the value.</param>
    /// <param name="decoder">The decoder used to parse the value.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes with <paramref name="encoder"/> and decodes with <paramref name="decoder"/>.</returns>
    public static Codec<T> Create<T>(IEncoder<T> encoder, IDecoder<T> decoder)
    {
        return new SimpleCodec<T>(encoder, decoder);
    }

    /// <summary>
    /// Creates a named a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> from the given <paramref name="encoder"/> and <paramref name="decoder"/>.
    /// </summary>
    /// <param name="encoder">The encoder used to serialize the value.</param>
    /// <param name="decoder">The decoder used to parse the value.</param>
    /// <param name="nameHolder">Holder of the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes with <paramref name="encoder"/>, decodes with <paramref name="decoder"/> and is named by <paramref name="nameHolder"/>.</returns>
    public static Codec<T> Create<T>(IEncoder<T> encoder, IDecoder<T> decoder, ValueHolder<string> nameHolder)
    {
        return new SimpleCodec<T>(encoder, decoder, nameHolder);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that ignores its input when encoding and always decodes the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to decode to.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a constant value.</returns>
    /// <remarks>
    /// the encoded form of the value is an empty map.
    /// </remarks>
    public static Codec<T> CreateUnit<T>(T value)
    {
        return new UnitCodec<T>(value);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that ignores its input when encoding and always decodes the value provided by <paramref name="value"/>.
    /// </summary>
    /// <param name="value">Provider of the value to decode to.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a constant value.</returns>
    /// <remarks>
    /// the encoded form of the value is an empty map. <paramref name="value"/> is only evaluated when the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> is used.
    /// </remarks>
    public static Codec<T> CreateUnit<T>(Provider<T> value)
    {
        return new UnitCodec<T>(value);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a pair of values.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the left value of the pair.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the right value of the pair.</param>
    /// <typeparam name="TLeft">The type of the left value of the pair.</typeparam>
    /// <typeparam name="TRight">The type of the right value of the pair.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes the left value followed by the right value and decodes them in the same order.</returns>
    public static Codec<(TLeft, TRight)> CreatePair<TLeft, TRight>(Codec<TLeft> left, Codec<TRight> right)
    {
        return new PairCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with <paramref name="left"/> and falls back to <paramref name="right"/> if the input cannot be decoded by the left a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> of the left alternative.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> of the right alternative.</param>
    /// <typeparam name="TLeft">The type of the value handled by <paramref name="left"/>.</typeparam>
    /// <typeparam name="TRight">The type of the value handled by <paramref name="right"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for an <see cref="T:DataFixerUpper.Utils.Either`2"/> value, wrapping the first alternative that succeeds.</returns>
    /// <remarks>
    /// Encoding uses the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> matching the side of the <see cref="T:DataFixerUpper.Utils.Either`2"/> value.
    /// </remarks>
    public static Codec<Either<TLeft, TRight>> CreateEither<TLeft, TRight>(Codec<TLeft> left, Codec<TRight> right)
    {
        return new EitherCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with either <paramref name="left"/> or <paramref name="right"/>, requiring exactly one of them to succeed.
    /// </summary>
    /// <param name="left">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> of the left alternative.</param>
    /// <param name="right">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> of the right alternative.</param>
    /// <typeparam name="TLeft">The type of the value handled by <paramref name="left"/>.</typeparam>
    /// <typeparam name="TRight">The type of the value handled by <paramref name="right"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for an <see cref="T:DataFixerUpper.Utils.Either`2"/> value of two mutually exclusive alternatives.</returns>
    /// <remarks>
    /// Decoding fails if both a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>s can decode the input, because the correct alternative cannot be determined.
    /// </remarks>
    public static Codec<Either<TLeft, TRight>> CreateXor<TLeft, TRight>(Codec<TLeft> left, Codec<TRight> right)
    {
        return new XorCodec<TLeft, TRight>(left, right);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes with <paramref name="primary"/> and falls back to <paramref name="alternative"/> when the primary a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> cannot decode the input.
    /// </summary>
    /// <param name="primary">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that is used first.</param>
    /// <param name="alternative">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that is used if <paramref name="primary"/> fails.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>s.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with <paramref name="primary"/>, or with <paramref name="alternative"/> if the primary fails.</returns>
    public static Codec<T> CreateAlternative<T>(Codec<T> primary, Codec<T> alternative)
    {
        return new AlternativeCodec<T>(primary, alternative);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes with <paramref name="primary"/> and falls back to <paramref name="alternative"/> when the primary a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> cannot decode the input.
    /// </summary>
    /// <param name="primary">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that is used first.</param>
    /// <param name="alternative">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that is used if <paramref name="primary"/> fails.</param>
    /// <param name="converter">The function that converts the alternative value to <typeparamref name="T"/>.</param>
    /// <typeparam name="T">The type of the value handled by <paramref name="primary"/>.</typeparam>
    /// <typeparam name="TAlt">The type of the value handled by <paramref name="alternative"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with <paramref name="primary"/>, or with <paramref name="alternative"/> if the primary fails.</returns>
    public static Codec<T> CreateAlternative<T, TAlt>(Codec<T> primary, Codec<TAlt> alternative, Func<TAlt, T> converter)
    {
        return new AlternativeCodec<T, TAlt>(primary, alternative, converter);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that serializes values of type <typeparamref name="T"/> as a string and parses them back from that string.
    /// </summary>
    /// <param name="toString">The function that converts a value to its string representation, or <see langword="null"/> if the value has no known name.</param>
    /// <param name="fromString">The function that converts a string back to a value, or <see langword="null"/> if the name is unknown.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that resolves values from their string representation.</returns>
    /// <remarks>
    /// Encoding fails if <paramref name="toString"/> returns <see langword="null"/>, and decoding fails if <paramref name="fromString"/> returns <see langword="null"/>.
    /// </remarks>
    public static Codec<T> CreateStringResolver<T>(
        Func<T, string?> toString,
        Func<string, T?> fromString
    )
    {
        return new StringResolverCodec<T>(toString, fromString);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that can refer to itself, allowing recursive data structures to be described.
    /// </summary>
    /// <param name="name">The name of the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>, used for diagnostics.</param>
    /// <param name="wrapped">The function that receives this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> and returns the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that is actually used for encoding and decoding.</param>
    /// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
    /// <returns>A recursive <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> named by <paramref name="name"/>.</returns>
    /// <remarks>
    /// <paramref name="wrapped"/> is applied lazily, so it may refer to the returned a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> while it is being built.
    /// </remarks>
    public static Codec<T> CreateRecursive<T>(string name, Func<Codec<T>, Codec<T>> wrapped)
    {
        return new RecursiveCodec<T>(name, wrapped);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a map with a fixed set of keys.
    /// </summary>
    /// <param name="keyCodec">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that handles the keys of the map.</param>
    /// <param name="valueCodec">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that handles the values of the map.</param>
    /// <param name="keys">The keys that the map may contain.</param>
    /// <param name="mutable"><see langword="true"/> to decode into a mutable dictionary; <see langword="false"/> to decode into an immutable dictionary.</param>
    /// <typeparam name="TKey">The type of the keys of the map.</typeparam>
    /// <typeparam name="TValue">The type of the values of the map.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a map using the given keys.</returns>
    public static MapCodec<IDictionary<TKey, TValue>> CreateSimpleDictionary<TKey, TValue>(Codec<TKey> keyCodec, Codec<TValue> valueCodec, IKeyable keys, bool mutable = false)
        where TKey : notnull
    {
        return new SimpleDictionaryCodec<TKey, TValue>(keyCodec, valueCodec, keys, mutable);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a map that may contain any key.
    /// </summary>
    /// <param name="keyCodec">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that handles the keys of the map.</param>
    /// <param name="valueCodec">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that handles the values of the map.</param>
    /// <param name="mutable"><see langword="true"/> to decode into a mutable dictionary; <see langword="false"/> to decode into an immutable dictionary.</param>
    /// <typeparam name="TKey">The type of the keys of the map.</typeparam>
    /// <typeparam name="TValue">The type of the values of the map.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a map of key-value pairs.</returns>
    public static Codec<IDictionary<TKey, TValue>> CreateUnboundedDictionary<TKey, TValue>(Codec<TKey> keyCodec, Codec<TValue> valueCodec, bool mutable = false)
        where TKey : notnull
    {
        return new UnboundedDictionaryCodec<TKey, TValue>(keyCodec, valueCodec, mutable);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a map whose values are handled by a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that depends on the key.
    /// </summary>
    /// <param name="keyCodec">The <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that handles the keys of the map.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the value of a given key.</param>
    /// <param name="mutable"><see langword="true"/> to decode into a mutable dictionary; <see langword="false"/> to decode into an immutable dictionary.</param>
    /// <typeparam name="TKey">The type of the keys of the map.</typeparam>
    /// <typeparam name="TValue">The type of the values of the map.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a map of key-value pairs, where the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> of each value is selected by <paramref name="dispatcher"/>.</returns>
    public static Codec<IDictionary<TKey, TValue>> CreateDispatchedDictionary<TKey, TValue>(Codec<TKey> keyCodec, Func<TKey, Codec<TValue>> dispatcher, bool mutable = false)
        where TKey : notnull
    {
        return new DispatchedDictionaryCodec<TKey, TValue>(keyCodec, dispatcher, mutable);
    }
}

/// <summary>
/// A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes values of type <typeparamref name="T"/> and decodes them back, combining an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> with an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
/// </summary>
/// <remarks>
/// the serialized form produced by a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> is not fixed by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> itself; it is decided by the <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> passed to the encoding and decoding operations.
/// </remarks>
/// <typeparam name="T">The type of the value handled by the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>
public abstract partial class Codec<T> : IEncoder<T>, IDecoder<T>
{
    /// <summary>
    /// Gets the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    public abstract ValueHolder<string> CodecNameHolder { get; }

    
    /// <inheritdoc/>
    public abstract DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull;

    /// <inheritdoc/>
    public abstract DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull;

    IEncoder<T> IEncoder<T>.WithLifecycle(Lifecycle lifecycle)
    {
        return WithLifecycle(lifecycle);
    }

    IDecoder<T> IDecoder<T>.WithLifecycle(Lifecycle lifecycle)
    {
        return WithLifecycle(lifecycle);
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.
    /// </summary>
    /// <returns>This <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
    public IEncoder<T> AsEncoder()
    {
        return this;
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>This <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
    public IDecoder<T> AsDecoder()
    {
        return this;
    }

    /// <inheritdoc/>
    public sealed override string ToString()
    {
        return CodecNameHolder.Value;
    }

    /// <summary>
    /// Returns the name of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> with the given <paramref name="transform"/> applied to it, in the form <c>Name[transform]</c>.
    /// </summary>
    /// <param name="transform">The transformation to apply to the name of the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</param>
    /// <returns>The transformed name of the <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
    public string ToString(string transform)
    {
        return ToString() + "[" + transform + "]";
    }

    /// <summary>
    /// Transforms the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> produced by the encoding and decoding operations of a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    public interface IResultMapper
    {
        /// <summary>
        /// Transforms the result of a decoding operation.
        /// </summary>
        /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
        /// <param name="input">The value that was decoded.</param>
        /// <param name="original">The result produced by the decoder.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>The transformed result.</returns>
        DataResult<(T, TObject?)> Apply<TObject>(DynamicOps<TObject> ops, TObject? input, DataResult<(T, TObject?)> original)
            where TObject : notnull;

        /// <summary>
        /// Transforms the result of an encoding operation.
        /// </summary>
        /// <param name="ops">The ops used to create the encoded value.</param>
        /// <param name="input">The value that was encoded.</param>
        /// <param name="original">The result produced by the encoder.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>The transformed result.</returns>
        DataResult<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, DataResult<TObject> original)
            where TObject : notnull;
    }
}
