using System;
using System.Collections.Immutable;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

// ReSharper disable once CheckNamespace
namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Extension for <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>
/// </summary>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>
public static class CodecExtension
{
    extension<T>(IEncoder<T> encoder)
    {
        /// <summary>
        /// Encodes the given <paramref name="input"/> into the form defined by <paramref name="ops"/>.
        /// </summary>
        /// <param name="ops">The ops used to create the encoded value.</param>
        /// <param name="input">The value to encode.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the encoded value, or an error if the value cannot be encoded.</returns>
        public DataResult<TObject> EncodeStart<TObject>(DynamicOps<TObject> ops, T input)
            where TObject : notnull
        {
            return encoder.Encode(input, ops, ops.Empty());
        }
        
        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>. to operate on a different type using the given mapping function.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> that first transforms the input using the mapping function, then encodes the result using this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.</returns>
        public IEncoder<TNewValue> CoMap<TNewValue>(Func<TNewValue, T> mapper)
        {
            return new CoMappedEncoder<TNewValue, T>(encoder, mapper);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> to operate on a different type using the given partial mapping function. Any errors the mapping function returns prevent further serialization.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> that first transforms the input using the mapping function, then encodes the result using this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.</returns>
        public IEncoder<TNewValue> FlatCoMap<TNewValue>(Func<TNewValue, DataResult<T>> mapper)
        {
            return new FlatCoMappedEncoder<TNewValue, T>(encoder, mapper);
        }
    }

    extension<T>(IDecoder<T> decoder)
    {
        /// <summary>
        /// Completely decodes the given serialized input and returns the decoded object in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
        /// </summary>
        /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
        /// <param name="input">The value to decode.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object.</returns>
        public DataResult<T> Parse<TObject>(DynamicOps<TObject> ops, TObject? input)
            where TObject : notnull
        {
            return decoder.Decode(ops, input).Map(result => result.Item1);
        }

        /// <summary>
        /// Decodes the input into an object and returns the decoded object and remaining serialized data in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
        /// </summary>
        /// <param name="dynamic">The serialized data.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object and the remaining serialized data.</returns>
        public DataResult<(T, TObject?)> Decode<TObject>(Dynamic<TObject> dynamic)
            where TObject : notnull

        {
            return decoder.Decode(dynamic.Ops, dynamic.Value);
        }
        
        /// <summary>
        /// Completely decodes an object from the given <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> data. Any remaining serialized data is discarded.
        /// </summary>
        /// <param name="dynamic">The serialized data.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object.</returns>
        public DataResult<T> Parse<TObject>(Dynamic<TObject> dynamic)
            where TObject : notnull
        {
            return decoder.Decode(dynamic.Ops, dynamic.Value).Map(result => result.Item1);
        }
        
        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> to operate on a different type using the given mapping function.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that decodes using this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>, then transforms the output using the mapping function.</returns>
        public IDecoder<TNewValue> Map<TNewValue>(Func<T, TNewValue> mapper)
        {
            return new MappedDecoder<T, TNewValue>(decoder, mapper);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> to operate on a different type using the given partial mapping function. Any errors the mapping function returns are merged with errors from deserialization.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the inner value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that decodes using this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>, then transforms the output using the mapping function.</returns>
        public IDecoder<TNewValue> FlatMap<TNewValue>(Func<T, DataResult<TNewValue>> mapper)
        {
            return new FlatMappedDecoder<T, TNewValue>(decoder, mapper);
        }
    }

    extension<T>(Codec<T> codec)
    {
        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> with <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/>.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that applies <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
        public Codec<T> Stable()
        {
            return codec.WithLifecycle(Lifecycle.Stable);
        }

        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> with <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/>.
        /// </summary>
        /// <param name="since">Deprecated version.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that applies <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
        public Codec<T> Deprecated(int since)
        {
            return codec.WithLifecycle(Lifecycle.CreateDeprecated(since));
        }
        
        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> using the given invertible mapping functions.
        /// </summary>
        /// <remarks>
        /// This method performs a <c>map</c> operation on both <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> and <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
        /// </remarks>
        /// <param name="toMapper">A function from this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type to the new type.</param>
        /// <param name="fromMapper">A function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the new type.</returns>
        public Codec<TNewValue> XMap<TNewValue>(Func<T, TNewValue> toMapper, Func<TNewValue, T> fromMapper)
        {
            return Codec.Create(
                codec.CoMap(fromMapper),
                codec.Map(toMapper),
                ValueHolder.Create(() => codec.ToString("XMapped"))
            );
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> using the given invertible partial function.
        /// </summary>
        /// <param name="toMapper">A partial function form this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type to the new type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <param name="fromMapper">A function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the new type.</returns>
        public Codec<TNewValue> CoMapFlatMap<TNewValue>(Func<T, DataResult<TNewValue>> toMapper, Func<TNewValue, T> fromMapper)
        {
            return Codec.Create(
                codec.CoMap(fromMapper), codec.FlatMap(toMapper),
                ValueHolder.Create(() => codec.ToString("CoMapFlatMapped"))
            );
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> using the given partially invertible function.
        /// </summary>
        /// <param name="toMapper">A function form this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type to the new type.</param>
        /// <param name="fromMapper">A partial function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the new type.</returns>
        public Codec<TNewValue> FlatCoMapMap<TNewValue>(Func<T, TNewValue> toMapper, Func<TNewValue, DataResult<T>> fromMapper)
        {
            return Codec.Create(
                codec.FlatCoMap(fromMapper), codec.Map(toMapper),
                ValueHolder.Create(() => codec.ToString("FlatCoMapMapped"))
            );
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> using the given partially invertible partial function.
        /// </summary>
        /// <remarks>
        /// This method performs a <c>flatMap</c> operation on both <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> and <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
        /// </remarks>
        /// <param name="toMapper">A partial function form this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type to the new type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <param name="fromMapper">A partial function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>'s type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for the new type.</returns>
        public Codec<TNewValue> FlatXMap<TNewValue>(Func<T, DataResult<TNewValue>> toMapper, Func<TNewValue, DataResult<T>> fromMapper)
        {
            return Codec.Create(
                codec.FlatCoMap(fromMapper), codec.FlatMap(toMapper),
                ValueHolder.Create(() => codec.ToString("XMapped"))
            );
        }
    }
}

/// <summary>
/// Extension for <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.
/// </summary>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>
public static class MapCodecExtension
{
    extension<T>(IMapEncoder<T> encoder)
    {
        /// <summary>
        /// Creates a new, empty <see cref="T:DataFixerUpper.Serialization.Collections.Builder.IRecordBuilder`1"/> that accepts values of the given serialized type.
        /// </summary>
        /// <remarks>
        /// The returned builder will used compressed keys if and only if the serialized type uses compressed keys.
        /// </remarks>
        /// <param name="ops">The ops used to create the encoded fields.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>Empty <see cref="T:DataFixerUpper.Serialization.Collections.Builder.IRecordBuilder`1"/>.</returns>
        public IRecordBuilder<TObject> GetCompressedBuilder<TObject>(DynamicOps<TObject> ops)
            where TObject : notnull
        {
            if (ops.CompressMaps())
            {
                return new CompressedRecordBuilder<TObject>(ops, encoder.GetCompressor(ops));
            }

            return ops.CreateMapBuilder();
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>. to operate on a different type using the given mapping function.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that first transforms the input using the mapping function, then encodes the result using this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.</returns>
        public IMapEncoder<TNewValue> CoMap<TNewValue>(Func<TNewValue, T> mapper)
        {
            return new CoMappedMapEncoder<TNewValue, T>(encoder, mapper);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> to operate on a different type using the given partial mapping function. Any errors the mapping function returns prevent further serialization.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that first transforms the input using the mapping function, then encodes the result using this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.</returns>
        public IMapEncoder<TNewValue> FlatCoMap<TNewValue>(Func<TNewValue, DataResult<T>> mapper)

        {
            return new FlatCoMappedMapEncoder<TNewValue, T>(encoder, mapper);
        }
    }

    extension<T>(IMapDecoder<T> decoder)
    {
        /// <summary>
        /// Decodes an object from the given serialized form. If the <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> uses compressed keys, the input should also contain compressed keys. 
        /// </summary>
        /// <remarks>
        /// Likewise, if the <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> does not use compressed keys, the input should also not contain compressed keys.
        /// </remarks>
        /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance defining the serialized form.</param>
        /// <param name="input">The serialized value that contains the record data to deserialize.</param>
        /// <typeparam name="TObject">The type of the serialized form.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object, or an error if no object could be decoded.</returns>
        public DataResult<T> CompressedDecode<TObject>(DynamicOps<TObject> ops, TObject? input)
            where TObject : notnull
        {
            if (!ops.CompressMaps())
            {
                return ops.GetMap(input).FlatMap(map => decoder.Decode(ops, map));
            }

            DataResult<ImmutableList<TObject?>> listResult = ops.GetList(input);
            if (!listResult.TryGetResult(out ImmutableList<TObject?>? list))
            {
                return DataResult.CreateError<T>("Input is not a list");
            }

            KeyCompressor<TObject> compressor = decoder.GetCompressor(ops);
            IMapLike<TObject> map = IMapLike<TObject>.ForCompressed(list, compressor);
            return decoder.Decode(ops, map);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> to operate on a different type using the given mapping function.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that decodes using this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>, then transforms the output using the mapping function.</returns>
        public IMapDecoder<TNewValue> Map<TNewValue>(Func<T, TNewValue> mapper)
        {
            return new MappedMapDecoder<T, TNewValue>(decoder, mapper);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> to operate on a different type using the given partial mapping function. Any errors the mapping function returns are merged with errors from deserialization.
        /// </summary>
        /// <param name="mapper">Transformation function.</param>
        /// <typeparam name="TNewValue">The type of the inner value returned by <paramref name="mapper"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that decodes using this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>, then transforms the output using the mapping function.</returns>
        public IMapDecoder<TNewValue> FlatMap<TNewValue>(Func<T, DataResult<TNewValue>> mapper)
        {
            return new FlatMappedMapDecoder<T, TNewValue>(decoder, mapper);
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> to operate on a different type using the mapping function decoded by <paramref name="dispatcher"/>.
        /// </summary>
        /// <param name="dispatcher">Decoder of transformation function.</param>
        /// <typeparam name="TNewValue">The type of the inner value returned by transformation function.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that decodes using this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>, then transforms the output using the decoded mapping function.</returns>
        public IMapDecoder<TNewValue> DispatchMap<TNewValue>(IMapDecoder<Func<T, TNewValue>> dispatcher)
        {
            return new DispatchMappedMapDecoder<T, TNewValue>(decoder, dispatcher);
        }
    }

    extension<T>(MapCodec<T> codec)
    {
        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> with <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/>.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that applies <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
        public MapCodec<T> Stable()
        {
            return codec.WithLifecycle(Lifecycle.Stable);
        }

        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> with <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/>.
        /// </summary>
        /// <param name="since">Deprecated version.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that applies <see cref="T:DataFixerUpper.Serialization.Lifecycle.Deprecated"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
        public MapCodec<T> Deprecated(int since)
        {
            return codec.WithLifecycle(Lifecycle.CreateDeprecated(since));
        }
        
        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> using the given partially invertible function.
        /// </summary>
        /// <remarks>
        /// This method performs a <c>map</c> operation on both <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> and <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.
        /// </remarks>
        /// <param name="toMapper">A function form this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>'s type to the new type.</param>
        /// <param name="fromMapper">A partial function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>'s type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the new type.</returns>
        public MapCodec<TNewValue> XMap<TNewValue>(Func<T, TNewValue> toMapper, Func<TNewValue, T> fromMapper)
        {
            return MapCodec.Create(
                codec.CoMap(fromMapper),
                codec.Map(toMapper),
                ValueHolder.Create(() => codec.ToString("XMapped"))
            );
        }

        /// <summary>
        /// Transforms this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> into another <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> using the given partially invertible partial function.
        /// </summary>
        /// <remarks>
        /// This method performs a <c>flatMap</c> operation on both <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> and <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.
        /// </remarks>
        /// <param name="toMapper">A partial function form this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>'s type to the new type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <param name="fromMapper">A partial function from the new type to this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>'s type. The value and any errors are wrapped in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="TNewValue">The new type.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the new type.</returns>
        public MapCodec<TNewValue> FlatXMap<TNewValue>(Func<T, DataResult<TNewValue>> toMapper, Func<TNewValue, DataResult<T>> fromMapper)
        {
            return MapCodec.Create(
                codec.FlatCoMap(fromMapper), codec.FlatMap(toMapper),
                ValueHolder.Create(() => codec.ToString("FlatXMapped"))
            );
        }
    }
}