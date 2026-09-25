using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

/// <summary>
/// Codec of primitive types.
/// </summary>
/// <typeparam name="T">Type of primitives.</typeparam>
public abstract class PrimitiveCodec<T> : Codec<T>
{
    /// <inheritdoc/>
    public override ValueHolder<string> CodecNameHolder => typeof(T).Name;

    /// <summary>
    /// Decodes a value of type <typeparamref name="T"/> from the given <paramref name="input"/>.
    /// </summary>
    /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
    /// <param name="input">The value to decode.</param>
    /// <typeparam name="TObject">The type of the decoded value.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded value.</returns>
    protected abstract DataResult<T> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull;

    /// <summary>
    /// Encodes the given <paramref name="value"/> into the form defined by <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the encoded value.</param>
    /// <param name="value">The value to encode.</param>
    /// <typeparam name="TObject">The type of the encoded value.</typeparam>
    /// <returns>Encoded value.</returns>
    protected abstract TObject Write<TObject>(DynamicOps<TObject> ops, T value)
        where TObject : notnull;

    /// <inheritdoc/>
    public override DataResult<TResult> Encode<TResult>(T input, DynamicOps<TResult> ops, TResult? prefix)
        where TResult : default
    {
        return ops.MergeToPrimitive(prefix, Write(ops, input));
    }

    /// <inheritdoc/>
    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return Read(ops, input).Map(value => (value, ops.Empty()));
    }
}

/// <summary>
/// Codec of numbers.
/// </summary>
/// <typeparam name="TNumber">Base type of number.</typeparam>
public class NumberCodec<TNumber> : PrimitiveCodec<TNumber>
    where TNumber : INumber<TNumber>
{
    /// <inheritdoc/>
    protected override DataResult<TNumber> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetNumberValue<TNumber>(input);
    }

    /// <inheritdoc/>
    protected override TObject Write<TObject>(DynamicOps<TObject> ops, TNumber value)
    {
        return ops.CreateNumber(value);
    }

    /// <summary>
    /// Create ranged number codec with given gange.
    /// </summary>
    /// <param name="min">Min value of the range.</param>
    /// <param name="max">Max value of the range.</param>
    /// <returns>Ranged number codec.</returns>
    public Codec<TNumber> Range(TNumber min, TNumber max)
    {
        return new ValidateCodec<TNumber>(this, ValidateRange);
        
        DataResult<TNumber> ValidateRange(TNumber value)
        {
            if (value < min || value > max)
            {
                return DataResult.CreateError<TNumber>($"Value {value} is out of range [{min}, {max}]");
            }

            return DataResult.CreateSuccess(value);
        }
    }
}

/// <summary>
/// Codec of <see langword="bool"/>.
/// </summary>
public sealed class BooleanCodec : PrimitiveCodec<bool>
{
    /// <inheritdoc/>
    protected override DataResult<bool> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetBoolValue(input);
    }

    /// <inheritdoc/>
    protected override TObject Write<TObject>(DynamicOps<TObject> ops, bool value)
    {
        return ops.CreateBoolValue(value);
    }
}

/// <summary>
/// Codec of <see langword="string"/>.
/// </summary>
public sealed class StringCodec : PrimitiveCodec<string>
{
    /// <inheritdoc/>
    protected override DataResult<string> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetStringValue(input);
    }

    /// <inheritdoc/>
    protected override TObject Write<TObject>(DynamicOps<TObject> ops, string value)
    {
        return ops.CreateString(value);
    }

    /// <summary>
    /// Create Codec for length limited string.
    /// </summary>
    /// <param name="minLength">Min length of string.</param>
    /// <param name="maxLength">Max length of string.</param>
    /// <returns>Length limited string codec.</returns>
    public Codec<string> LengthLimited(int minLength = 0, int maxLength = int.MaxValue)
    {
        return new ValidateCodec<string>(this, ValidateLength);

        DataResult<string> ValidateLength(string s)
        {
            if (s.Length < minLength)
            {
                
                return DataResult.CreateError<string>($"String {s} is too short: {s.Length}, expected range [{minLength}-{maxLength}]");
            }
            
            if (s.Length > maxLength)
            {
                return DataResult.CreateError<string>($"String {s} is too long: {s.Length}, expected range [{minLength}-{maxLength}]");
            }

            return DataResult.CreateSuccess(s);
        }
    }

    /// <summary>
    /// Create Codec for regex-validated string.
    /// </summary>
    /// <param name="regex">Regex to test string.</param>
    /// <returns>Regex-validated string codec.</returns>
    public Codec<string> Regex(Regex regex)
    {
        return new ValidateCodec<string>(this, ValidateRegex);

        DataResult<string> ValidateRegex(string s)
        {
            if (regex.IsMatch(s))
            {
                return DataResult.CreateSuccess(s);
            }

            return DataResult.CreateError<string>($"String {s} not match regex {regex}");
        }
    }
}

/// <summary>
/// Codec of <see cref="T:System.IO.Stream"/>.
/// </summary>
public sealed class StreamCodec : PrimitiveCodec<Stream>
{
    /// <inheritdoc/>
    protected override DataResult<Stream> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetStream(input);
    }

    /// <inheritdoc/>
    protected override TObject Write<TObject>(DynamicOps<TObject> ops, Stream value)
    {
        return ops.CreateStream(value);
    }
}

/// <summary>
/// Codec of list of numbers.
/// </summary>
public sealed class NumberListValuesCodec<TNumber> : PrimitiveCodec<IEnumerable<TNumber>>
    where TNumber : INumber<TNumber>
{
    /// <inheritdoc/>
    public override ValueHolder<string> CodecNameHolder => $"{typeof(TNumber)}List";

    /// <inheritdoc/>
    protected override DataResult<IEnumerable<TNumber>> Read<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetNumberList<TObject, TNumber>(input);
    }

    /// <inheritdoc/>
    protected override TObject Write<TObject>(DynamicOps<TObject> ops, IEnumerable<TNumber> value)
    {
        return ops.CreateNumberList(value);
    }
}