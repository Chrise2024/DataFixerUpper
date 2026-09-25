using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Utils;

/// <summary>
/// OrDefault mapper for <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
/// </summary>
/// <param name="ValueHolder">Default value.</param>
/// <param name="OnError">Operation on error message.</param>
/// <typeparam name="T">The type of the value handled by the codec.</typeparam>
public sealed record CodecOrDefaultMapper<T>(ValueHolder<T> ValueHolder, UnaryOperation<string> OnError) : Codec<T>.IResultMapper
{
    /// <summary>
    /// Construct from default value.
    /// </summary>
    /// <param name="valueHolder">Default value.</param>
    public CodecOrDefaultMapper(ValueHolder<T> valueHolder) : this(valueHolder, Functions.Identity) { }
    
    /// <summary>
    /// Construct from default value and error handler.
    /// </summary>
    /// <param name="value">Default value.</param>
    /// <param name="onError">Operation on error message.</param>
    public CodecOrDefaultMapper(T value, UnaryOperation<string> onError) : this(Utils.ValueHolder.Create(value), onError) { }
    
    /// <summary>
    /// Construct from default value.
    /// </summary>
    /// <param name="value">Default value.</param>
    public CodecOrDefaultMapper(T value) : this(value, Functions.Identity) { }

    /// <inheritdoc/>
    public DataResult<(T, TObject?)> Apply<TObject>(DynamicOps<TObject> ops, TObject? input, DataResult<(T, TObject?)> original)
        where TObject : notnull
    {
        return DataResult.CreateSuccess(original.MapError(OnError).GetResultOrDefault((ValueHolder.Value, input)));
    }

    /// <inheritdoc/>
    public DataResult<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, DataResult<TObject> original)
        where TObject : notnull
    {
        return original.MapError(OnError);
    }

    /// <ihenheritdoc />
    public override string ToString()
    {
        return $"OrDefault[{ValueHolder.Value}]";
    }
}

/// <summary>
/// OrDefault mapper for <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.
/// </summary>
/// <param name="ValueHolder">Default value.</param>
/// <param name="OnError">Operation on error message.</param>
/// <typeparam name="T">The type of the value handled by the codec.</typeparam>
public sealed record MapCodecOrDefaultMapper<T>(ValueHolder<T> ValueHolder, UnaryOperation<string> OnError) : MapCodec<T>.IResultMapper
{
    /// <summary>
    /// Construct from default value.
    /// </summary>
    /// <param name="valueHolder">Default value.</param>
    public MapCodecOrDefaultMapper(ValueHolder<T> valueHolder) : this(valueHolder, Functions.Identity) { }
    
    /// <summary>
    /// Construct from default value and error handler.
    /// </summary>
    /// <param name="value">Default value.</param>
    /// <param name="onError">Operation on error message.</param>
    public MapCodecOrDefaultMapper(T value, UnaryOperation<string> onError) : this(Utils.ValueHolder.Create(value), onError) { }
   
    /// <summary>
    /// Construct from default value.
    /// </summary>
    /// <param name="value">Default value.</param>
    public MapCodecOrDefaultMapper(T value) : this(value, Functions.Identity) { }

    /// <inheritdoc/>
    public DataResult<T> Apply<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input, DataResult<T> original)
        where TObject : notnull
    {
        return DataResult.CreateSuccess(original.MapError(OnError).GetResultOrDefault(ValueHolder.Value));
    }

    /// <inheritdoc/>
    public IRecordBuilder<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, IRecordBuilder<TObject> original)
        where TObject : notnull
    {
        return original.MapError(OnError);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"OrDefault[{ValueHolder.Value}]";
    }
}

/// <summary>
/// SetPartial mapper for <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>.
/// </summary>
/// <param name="PartialHolder">Partial value.</param>
public sealed record CodecSetPartialMapper<T>(ValueHolder<T> PartialHolder) : Codec<T>.IResultMapper
{
    /// <inheritdoc/>
    public DataResult<(T, TObject?)> Apply<TObject>(DynamicOps<TObject> ops, TObject? input, DataResult<(T, TObject?)> original)
        where TObject : notnull
    {
        return original.SetPartial((PartialHolder.Value, input));
    }

    /// <inheritdoc/>
    public DataResult<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, DataResult<TObject> original)
        where TObject : notnull
    {
        return original;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"SetPartial[{PartialHolder.Value}]";
    }
}

/// <summary>
/// SetPartial mapper for <see cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>.
/// </summary>
/// <param name="PartialHolder">Partial value.</param>
public sealed record MapCodecSetPartialMapper<T>(ValueHolder<T> PartialHolder) : MapCodec<T>.IResultMapper
{
    /// <inheritdoc/>
    public DataResult<T> Apply<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input, DataResult<T> original)
        where TObject : notnull
    {
        return original.SetPartial(PartialHolder);
    }

    /// <inheritdoc/>
    public IRecordBuilder<TObject> CoApply<TObject>(DynamicOps<TObject> ops, T input, IRecordBuilder<TObject> original)
        where TObject : notnull
    {
        return original;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"SetPartial[{PartialHolder.Value}]";
    }
}