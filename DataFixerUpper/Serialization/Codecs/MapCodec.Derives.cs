using System;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

public abstract partial class MapCodec<T>
{
    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the results.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public MapCodec<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleMapCodec<T>(this, lifecycle);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that transforms the results of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The mapper that transforms the results of the encoding and decoding operations.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that applies <paramref name="mapper"/> to the results of this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</returns>
    public MapCodec<T> MapResult(IResultMapper mapper)
    {
        return new ResultMappedMapCodec<T>(this, mapper);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes to <paramref name="value"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="value">The value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails.</param>
    /// <param name="onError">The function that transforms the error message of the failed result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that falls back to <paramref name="value"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public MapCodec<T> OrDefault(T value, UnaryOperation<string> onError)
    {
        return new ResultMappedMapCodec<T>(this, new MapCodecOrDefaultMapper<T>(value, onError));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes to <paramref name="value"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="value">The value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that falls back to <paramref name="value"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public MapCodec<T> OrDefault(T value)
    {
        return new ResultMappedMapCodec<T>(this, new MapCodecOrDefaultMapper<T>(value));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes to the value of <paramref name="valueHolder"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="valueHolder">Holder of the value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails.</param>
    /// <param name="onError">The function that transforms the error message of the failed result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that falls back to the value of <paramref name="valueHolder"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public MapCodec<T> OrDefault(ValueHolder<T> valueHolder, UnaryOperation<string> onError)
    {
        return new ResultMappedMapCodec<T>(this, new MapCodecOrDefaultMapper<T>(valueHolder, onError));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that decodes to the value of <paramref name="valueHolder"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="valueHolder">Holder of the value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> fails.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that falls back to the value of <paramref name="valueHolder"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public MapCodec<T> OrDefault(ValueHolder<T> valueHolder)
    {
        return new ResultMappedMapCodec<T>(this, new MapCodecOrDefaultMapper<T>(valueHolder));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that reports the value of <paramref name="valueHolder"/> as the partial result of a decoding operation.
    /// </summary>
    /// <param name="valueHolder">Holder of the partial result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that sets <paramref name="valueHolder"/> as its partial result.</returns>
    public MapCodec<T> SetPartial(ValueHolder<T> valueHolder)
    {
        return new ResultMappedMapCodec<T>(this, new MapCodecSetPartialMapper<T>(valueHolder));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TElem">The type of the value handled by the resulting <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is used to encode and decode the type of the value from the same fields.
    /// </remarks>
    public MapCodec<TElem> Dispatch<TElem>(Func<TElem, T> typeSelector, Func<T, MapCodec<TElem>> dispatcher)
    {
        return PartialDispatch(typeSelector.Then(DataResult.CreateSuccess), dispatcher.Then(DataResult.CreateSuccess));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, using <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> as the lifecycle of the type value.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TElem">The type of the value handled by the resulting <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is used to encode and decode the type of the value from the same fields.
    /// </remarks>
    public MapCodec<TElem> DispatchStable<TElem>(Func<TElem, T> typeSelector, Func<T, MapCodec<TElem>> dispatcher)
    {
        return PartialDispatch(typeSelector.Then(e => DataResult.CreateSuccess(e, Lifecycle.Stable)), dispatcher.Then(c => DataResult.CreateSuccess(c, Lifecycle.Stable)));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, where selecting the type and the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> may fail.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value, which may fail.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value, which may fail.</param>
    /// <typeparam name="TElem">The type of the value handled by the resulting <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// This <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> is used to encode and decode the type of the value from the same fields.
    /// </remarks>
    public MapCodec<TElem> PartialDispatch<TElem>(Func<TElem, DataResult<T>> typeSelector, Func<T, DataResult<MapCodec<TElem>>> dispatcher)
    {
        return new TypeDispatchMapCodec<T, TElem>(this, typeSelector, dispatcher);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that validates values with the given <paramref name="validator"/>.
    /// </summary>
    /// <param name="validator">The function that validates a value, returning a successful result with the validated value or an error.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that validates values before encoding them and after decoding them.</returns>
    public MapCodec<T> Validate(Func<T, DataResult<T>> validator)
    {
        return new ValidateMapCodec<T>(this, validator);
    }
}
