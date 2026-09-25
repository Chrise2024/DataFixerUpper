using System;
using System.Collections.Generic;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

public abstract partial class Codec<T>
{
    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the results.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
    public Codec<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleCodec<T>(this, lifecycle);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for an immutable list of the values handled by this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    /// <param name="minSize">The minimum number of elements the list may contain.</param>
    /// <param name="maxSize">The maximum number of elements the list may contain.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for an immutable list.</returns>
    /// <remarks>
    /// Encoding and decoding fail if the size of the list is outside the range defined by <paramref name="minSize"/> and <paramref name="maxSize"/>.
    /// </remarks>
    public Codec<IList<T>> ImmutableList(int minSize = 0, int maxSize = int.MaxValue)
    {
        return new ListCodec<T>(this, minSize, maxSize);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a mutable list of the values handled by this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    /// <param name="minSize">The minimum number of elements the list may contain.</param>
    /// <param name="maxSize">The maximum number of elements the list may contain.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a mutable list.</returns>
    /// <remarks>
    /// Encoding and decoding fail if the size of the list is outside the range defined by <paramref name="minSize"/> and <paramref name="maxSize"/>. Unlike <c>ImmutableList</c>, the decoded list is mutable.
    /// </remarks>
    public Codec<IList<T>> List(int minSize = 0, int maxSize = int.MaxValue)
    {
        return new ListCodec<T>(this, minSize, maxSize, true);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for a fixed-size array of the values handled by this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
    /// </summary>
    /// <param name="length">The number of elements the array must contain.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> for an array.</returns>
    /// <remarks>
    /// Encoding and decoding fail if the array does not contain exactly <paramref name="length"/> elements.
    /// </remarks>
    public Codec<T[]> Array(int length)
    {
        return new ArrayCodec<T>(this, length);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> that encodes and decodes this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> as the field named <paramref name="name"/> of a map.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for the named field.</returns>
    public MapCodec<T> Field(string name)
    {
        return MapCodec.Create(
            AsEncoder().Field(name),
            AsDecoder().Field(name),
            $"Field[{name}:{ToString()}]"
        );
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an optional field named <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="lenient"><see langword="true"/> to decode a field that cannot be parsed to <paramref name="defaultValue"/> instead of failing.</param>
    /// <param name="fieldLifecycle">The lifecycle of a value that was decoded from the field.</param>
    /// <param name="defaultValue">The value to decode to if the field is missing or cannot be parsed.</param>
    /// <param name="defaultLifecycle">The lifecycle of <paramref name="defaultValue"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an optional field.</returns>
    /// <remarks>
    /// The field is only encoded when the value has a value, and it decodes to <paramref name="defaultValue"/> if it is missing from the input.
    /// </remarks>
    public MapCodec<Optional<T>> OptionalField(string name, bool lenient, Lifecycle fieldLifecycle, Optional<T> defaultValue, Lifecycle defaultLifecycle)
    {
        return new OptionalFieldCodec<T>(name, this, lenient, fieldLifecycle, defaultValue, defaultLifecycle);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an optional field named <paramref name="name"/> that uses <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> as its lifecycle.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="lenient"><see langword="true"/> to decode a field that cannot be parsed to <paramref name="defaultValue"/> instead of failing.</param>
    /// <param name="defaultValue">The value to decode to if the field is missing or cannot be parsed.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for an optional field.</returns>
    /// <remarks>
    /// The field is only encoded when the value has a value, and it decodes to <paramref name="defaultValue"/> if it is missing from the input.
    /// </remarks>
    public MapCodec<Optional<T>> OptionalField(string name, bool lenient = false, Optional<T> defaultValue = default)
    {
        return new OptionalFieldCodec<T>(name, this, lenient, defaultValue);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that transforms the results of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The mapper that transforms the results of the encoding and decoding operations.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that applies <paramref name="mapper"/> to the results of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.</returns>
    public Codec<T> MapResult(IResultMapper mapper)
    {
        return new ResultMappedCodec<T>(this, mapper);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes to <paramref name="value"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="value">The value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <param name="onError">The function that transforms the error message of the failed result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to <paramref name="value"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public Codec<T> OrDefault(T value, UnaryOperation<string> onError)
    {
        return new ResultMappedCodec<T>(this, new CodecOrDefaultMapper<T>(value, onError));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes to <paramref name="value"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="value">The value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to <paramref name="value"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public Codec<T> OrDefault(T value)
    {
        return new ResultMappedCodec<T>(this, new CodecOrDefaultMapper<T>(value));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes to the value of <paramref name="valueHolder"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="valueHolder">Holder of the value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <param name="onError">The function that transforms the error message of the failed result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to the value of <paramref name="valueHolder"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public Codec<T> OrDefault(ValueHolder<T> valueHolder, UnaryOperation<string> onError)
    {
        return new ResultMappedCodec<T>(this, new CodecOrDefaultMapper<T>(valueHolder, onError));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes to the value of <paramref name="valueHolder"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails to decode the input.
    /// </summary>
    /// <param name="valueHolder">Holder of the value to decode to if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to the value of <paramref name="valueHolder"/>.</returns>
    /// <remarks>
    /// Only the result of a decoding operation is replaced, so an error that occurs while encoding is still reported.
    /// </remarks>
    public Codec<T> OrDefault(ValueHolder<T> valueHolder)
    {
        return new ResultMappedCodec<T>(this, new CodecOrDefaultMapper<T>(valueHolder));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that promotes a partial result of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> to a successful result.
    /// </summary>
    /// <param name="onError">The action to perform with the error message when a partial result is promoted.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes to the partial result of this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> when one is available.</returns>
    public Codec<T> PromptPartial(Consumer<string> onError)
    {
        return Codec.Create(this, AsDecoder().PromptPartial(onError));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that reports the value of <paramref name="valueHolder"/> as the partial result of a decoding operation.
    /// </summary>
    /// <param name="valueHolder">Holder of the partial result.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that sets <paramref name="valueHolder"/> as its partial result.</returns>
    public Codec<T> SetPartial(ValueHolder<T> valueHolder)
    {
        return new ResultMappedCodec<T>(this, new CodecSetPartialMapper<T>(valueHolder));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to <paramref name="alternative"/> when this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> cannot decode the input.
    /// </summary>
    /// <param name="alternative">The codec that is used if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>, or with <paramref name="alternative"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</returns>
    public Codec<T> WithAlternative(Codec<T> alternative)
    {
        return new AlternativeCodec<T>(this, alternative);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that falls back to <paramref name="alternative"/> when this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> cannot decode the input.
    /// </summary>
    /// <param name="alternative">The codec that is used if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</param>
    /// <param name="converter">The function that converts the alternative value to <typeparamref name="T"/>.</param>
    /// <typeparam name="TAlt">The type of the value handled by <paramref name="alternative"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that decodes with this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>, or with <paramref name="alternative"/> if this <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> fails.</returns>
    public Codec<T> WithAlternative<TAlt>(Codec<TAlt> alternative, Func<TAlt, T> converter)
    {
        return new AlternativeCodec<T, TAlt>(this, alternative, converter);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// The type value is stored in the field named <c>type</c>.
    /// </remarks>
    public Codec<TInstance> Dispatch<TInstance>(Func<TInstance, T> typeSelector, Func<T, MapCodec<TInstance>> dispatcher)
    {
        return PartialDispatch(typeSelector.Then(DataResult.CreateSuccess), dispatcher.Then(DataResult.CreateSuccess));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value.
    /// </summary>
    /// <param name="typeKey">The name of the field that stores the type value.</param>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    public Codec<TInstance> Dispatch<TInstance>(string typeKey, Func<TInstance, T> typeSelector, Func<T, MapCodec<TInstance>> dispatcher)
    {
        return PartialDispatch(typeKey, typeSelector.Then(DataResult.CreateSuccess), dispatcher.Then(DataResult.CreateSuccess));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, using <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> as the lifecycle of the type value.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// The type value is stored in the field named <c>type</c>.
    /// </remarks>
    public Codec<TInstance> DispatchStable<TInstance>(Func<TInstance, T> typeSelector, Func<T, MapCodec<TInstance>> dispatcher)
    {
        return PartialDispatch(typeSelector.Then(e => DataResult.CreateSuccess(e, Lifecycle.Stable)), dispatcher.Then(c => DataResult.CreateSuccess(c, Lifecycle.Stable)));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, using <see cref="P:DataFixerUpper.Serialization.Lifecycle.Stable"/> as the lifecycle of the type value.
    /// </summary>
    /// <param name="typeKey">The name of the field that stores the type value.</param>
    /// <param name="typeSelector">The function that extracts the type value from a value.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    public Codec<TInstance> DispatchStable<TInstance>(string typeKey, Func<TInstance, T> typeSelector, Func<T, MapCodec<TInstance>> dispatcher)
    {
        return PartialDispatch(typeKey, typeSelector.Then(e => DataResult.CreateSuccess(e, Lifecycle.Stable)), dispatcher.Then(c => DataResult.CreateSuccess(c, Lifecycle.Stable)));
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, where selecting the type and the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> may fail.
    /// </summary>
    /// <param name="typeSelector">The function that extracts the type value from a value, which may fail.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value, which may fail.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    /// <remarks>
    /// The type value is stored in the field named <c>type</c>.
    /// </remarks>
    public Codec<TInstance> PartialDispatch<TInstance>(Func<TInstance, DataResult<T>> typeSelector, Func<T, DataResult<MapCodec<TInstance>>> dispatcher)
    {
        return new TypeDispatchMapCodec<T, TInstance>(this, typeSelector, dispatcher).AsCodec();
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that dispatches to a <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> selected by the type of the value, where selecting the type and the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> may fail.
    /// </summary>
    /// <param name="typeKey">The name of the field that stores the type value.</param>
    /// <param name="typeSelector">The function that extracts the type value from a value, which may fail.</param>
    /// <param name="dispatcher">The function that returns the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> for a type value, which may fail.</param>
    /// <typeparam name="TInstance">The type of the value handled by the resulting codec.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that encodes and decodes values with the <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/> returned by <paramref name="dispatcher"/>.</returns>
    public Codec<TInstance> PartialDispatch<TInstance>(string typeKey, Func<TInstance, DataResult<T>> typeSelector, Func<T, DataResult<MapCodec<TInstance>>> dispatcher)
    {
        return new TypeDispatchMapCodec<T, TInstance>(Field(typeKey), typeSelector, dispatcher).AsCodec();
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that validates values with the given <paramref name="validator"/>.
    /// </summary>
    /// <param name="validator">The function that validates a value, returning a successful result with the validated value or an error.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> that validates values before encoding them and after decoding them.</returns>
    public Codec<T> Validate(Func<T, DataResult<T>> validator)
    {
        return new ValidateCodec<T>(this, validator);
    }
}
