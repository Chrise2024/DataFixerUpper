using System;
using System.Collections.Generic;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// A <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/> that wraps the result of reading a value that may not be present.
/// </summary>
/// <remarks>
/// Every operation is only performed when the wrapped value was read successfully; otherwise the operation fails with the error of that read. This allows a chain of operations to be composed without checking for a missing value at each step.
/// </remarks>
/// <param name="ops">The ops used to read and modify the wrapped value.</param>
/// <param name="delegate">The result of the read that produced this instance.</param>
/// <typeparam name="TObject">The type this class serializes to and deserializes from, for example <see cref="T:System.Text.Json.Nodes.JsonNode"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>
public sealed class OptionalDynamic<TObject>(DynamicOps<TObject> ops, DataResult<Dynamic<TObject>> @delegate)
    : DynamicLike<TObject>(ops)
    where TObject : notnull
{
    private readonly DataResult<Dynamic<TObject>> _delegate = @delegate;
    
    /// <summary>
    /// Gets the wrapped value, or <see langword="null"/> if the wrapped read failed.
    /// </summary>
    public Optional<Dynamic<TObject>> Result => _delegate.GetResult();
    
    /// <summary>
    /// Gets the result of the read that produced this instance.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the wrapped value, or an error if the read failed.</returns>
    public DataResult<Dynamic<TObject>> Get()
    {
        return _delegate;
    }
    
    /// <summary>
    /// Projects the wrapped value with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that produces the mapped value.</param>
    /// <typeparam name="T1">The type of the value returned by <paramref name="mapper"/>.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the mapped value, or an error if the read failed.</returns>
    public DataResult<T1> Map<T1>(Func<Dynamic<TObject>, T1> mapper)
    {
        return _delegate.Map(mapper);
    }
    
    /// <summary>
    /// Projects the wrapped value with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that produces the mapped result.</param>
    /// <typeparam name="T1">The type of the value contained in the result of <paramref name="mapper"/>.</typeparam>
    /// <returns>The result produced by <paramref name="mapper"/>, or an error if the read failed.</returns>
    public DataResult<T1> FlatMap<T1>(Func<Dynamic<TObject>, DataResult<T1>> mapper)
    {
        return _delegate.FlatMap(mapper);
    }

    /// <summary>
    /// Gets the wrapped value, or an empty list if the wrapped read failed.
    /// </summary>
    /// <returns>The wrapped value, or an empty list created in the format of the ops of this instance.</returns>
    public Dynamic<TObject> OrDefaultEmptyList()
    {
        return Result.GetOrDefault(this.CreateList());
    }
    
    /// <summary>
    /// Gets the wrapped value, or an empty map if the wrapped read failed.
    /// </summary>
    /// <returns>The wrapped value, or an empty map created in the format of the ops of this instance.</returns>
    public Dynamic<TObject> OrDefaultEmptyMap()
    {
        return Result.GetOrDefault(this.CreateMap());
    }
    
    /// <summary>
    /// Invokes <paramref name="action"/> with the wrapped value.
    /// </summary>
    /// <param name="action">The function to invoke with the wrapped value.</param>
    /// <typeparam name="T1">The type of the value returned by <paramref name="action"/>.</typeparam>
    /// <returns>The result of <paramref name="action"/>, or an error if the read failed.</returns>
    public DataResult<T1> Into<T1>(Func<Dynamic<TObject>, T1> action)
    {
        return _delegate.Map(action);
    }

    /// <inheritdoc/>
    public override DataResult<(TResult, TObject?)> Decode<TResult>(IDecoder<TResult> decoder)
    {
        return _delegate.FlatMap(dynamic => dynamic.Decode(decoder));
    }

    /// <inheritdoc/>
    public override DataResult<TNumber> AsNumber<TNumber>()
    {
        return _delegate.FlatMap(dynamic => dynamic.AsNumber<TNumber>());
    }

    /// <inheritdoc/>
    public override DataResult<string> AsString()
    {
        return _delegate.FlatMap(dynamic => dynamic.AsString());
    }

    /// <inheritdoc/>
    public override DataResult<bool> AsBool()
    {
        return _delegate.FlatMap(dynamic => dynamic.AsBool());
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<Dynamic<TObject>>> AsListValues()
    {
        return _delegate.FlatMap(dynamic => dynamic.AsListValues());
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>>> AsMapEntries()
    {
        return _delegate.FlatMap(dynamic => dynamic.AsMapEntries());
    }

    /// <inheritdoc/>
    public override OptionalDynamic<TObject> Get(string key)
    {
        return new OptionalDynamic<TObject>(Ops, _delegate.FlatMap(dynamic => dynamic.Get(key)._delegate));
    }

    /// <inheritdoc/>
    public override DataResult<TObject> Get(TObject key)
    {
        return _delegate.FlatMap(dynamic => dynamic.Get(key));
    }
}
