using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// Operations for dotnet universal objects.
/// </summary>
public sealed class DotnetOps : DynamicOps<object>
{
    /// <summary>
    /// Instance of <see cref="T:DataFixerUpper.Serialization.DynamicOps.DotnetOps"/>.
    /// </summary>
    public static readonly DotnetOps Instance = new();

    private DotnetOps() { }

    /// <inheritdoc/>
    public override object? Empty()
    {
        return null;
    }

    /// <inheritdoc/>
    public override object EmptyMap()
    {
        return new Hashtable(0);
    }

    /// <inheritdoc/>
    public override object EmptyList()
    {
        return new ArrayList(0);
    }

    /// <inheritdoc/>
    public override TOther? ConvertTo<TOther>(DynamicOps<TOther> otherOp, object? input)
        where TOther : default
    {
        if (input is null)
        {
            return otherOp.Empty();
        }

        return input switch
        {
            string str => otherOp.CreateString(str),
            bool bl => otherOp.CreateBoolValue(bl),
            byte b => otherOp.CreateByte(b),
            _ => input.GetType().IsPrimitive ? ConvertNumber(otherOp, input) : ConvertManaged(otherOp, input)
        };
    }

    /// <inheritdoc/>
    public override DataResult<byte> GetByteValue(object? input)
    {
        if (input is byte b)
        {
            return DataResult.CreateSuccess(b);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToByte(input));
        }
        catch
        {
            return DataResult.CreateError<byte>($"Input not a byte: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateByte(byte value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override DataResult<short> GetShortValue(object? input)
    {
        if (input is short s)
        {
            return DataResult.CreateSuccess(s);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToInt16(input));
        }
        catch
        {
            return DataResult.CreateError<short>($"Input not a short: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateShort(short value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override DataResult<int> GetIntValue(object? input)
    {
        if (input is int i)
        {
            return DataResult.CreateSuccess(i);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToInt32(input));
        }
        catch
        {
            return DataResult.CreateError<int>($"Input not a int: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateInt(int value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override DataResult<long> GetLongValue(object? input)
    {
        if (input is long l)
        {
            return DataResult.CreateSuccess(l);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToInt64(input));
        }
        catch
        {
            return DataResult.CreateError<long>($"Input not a long: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateLong(long value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override DataResult<float> GetFloatValue(object? input)
    {
        if (input is float f)
        {
            return DataResult.CreateSuccess(f);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToSingle(input));
        }
        catch
        {
            return DataResult.CreateError<float>($"Input not a float: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateFloat(float value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override DataResult<double> GetDoubleValue(object? input)
    {
        if (input is double d)
        {
            return DataResult.CreateSuccess(d);
        }

        try
        {
            return DataResult.CreateSuccess(Convert.ToDouble(input));
        }
        catch
        {
            return DataResult.CreateError<double>($"Input not a double: {input}");
        }
    }

    /// <inheritdoc/>
    public override object CreateDouble(double value)
    {
        return value;
    }

    /// <inheritdoc/>
    public override object CreateStream(Stream stream)
    {
        return stream;
    }

    /// <inheritdoc/>
    public override object CreateIntList(IEnumerable<int> numbers)
    {
        return numbers.ToImmutableList();
    }

    /// <inheritdoc/>
    public override object CreateLongList(IEnumerable<long> numbers)
    {
        return numbers.ToImmutableList();
    }

    /// <inheritdoc/>
    public override DataResult<string> GetStringValue(object? @string)
    {
        if (@string is string str)
        {
            return DataResult.CreateSuccess(str);
        }

        return DataResult.CreateError<string>($"Not a string: {@string}");
    }

    /// <inheritdoc/>
    public override object CreateString(string @string)
    {
        return @string;
    }

    /// <inheritdoc/>
    public override DataResult<bool> GetBoolValue(object? @bool)
    {
        if (@bool is bool bl)
        {
            return DataResult.CreateSuccess(bl);
        }

        return DataResult.CreateError<bool>($"Not a bool: {@bool}");
    }

    /// <inheritdoc/>
    public override object CreateBoolValue(bool @bool)
    {
        return @bool;
    }

    /// <inheritdoc/>
    public override object CreateList(IEnumerable<object?> list)
    {
        return list.ToImmutableList();
    }

    /// <inheritdoc/>
    public override object CreateMap(IEnumerable<KeyValuePair<object, object?>> entries)
    {
        return entries.ToImmutableDictionary();
    }

    /// <inheritdoc/>
    public override object CreateMap(IEnumerable<KeyValuePair<string, object?>> entries)
    {
        return entries.ToImmutableDictionary();
    }

    /// <inheritdoc/>
    public override RecordBuilder<object> CreateMapBuilder()
    {
        return new FixedDictionaryBuilder(this);
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToList(object? list, object? other)
    {
        if (list is IList il)
        {
            return DataResult.CreateSuccess<object>(il.Cast<object>().Append(other).ToImmutableList());
        }

        if (list is null)
        {
            return DataResult.CreateSuccess<object>(ImmutableList.Create(other));
        }

        return DataResult.CreateError($"{nameof(MergeToList)} called with not a list: {list}", Optional.Create(list));
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToList(object? list, IEnumerable<object?> values)
    {
        if (list is IList il)
        {
            return DataResult.CreateSuccess<object>(il.Cast<object>().Concat(values).ToImmutableList());
        }

        if (list is null)
        {
            return DataResult.CreateSuccess<object>(ImmutableList.CreateRange(values));
        }

        return DataResult.CreateError($"{nameof(MergeToList)} called with not a list: {list}", Optional.Create(list));
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToMap(object? dict, object key, object? value)
    {
        if (dict is IDictionary id)
        {
            ImmutableDictionary<object, object?>.Builder builder = ImmutableDictionary.CreateBuilder<object, object?>();
            builder.AddRange(GetDictionaryEntries(id));
            builder.Add(key, value);
            return DataResult.CreateSuccess<object>(builder.ToImmutable());
        }

        if (dict is null)
        {
            ImmutableDictionary<object, object?>.Builder builder = ImmutableDictionary.CreateBuilder<object, object?>();
            builder.Add(key, value);
            return DataResult.CreateSuccess<object>(builder.ToImmutable());
        }

        return DataResult.CreateError($"{nameof(MergeToMap)} called with not a dict: {dict}", Optional.Create(dict));
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToMap(object? dict, string key, object? value)
    {
        return MergeToMap(dict, (object) key, value);
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToMap(object? dict, IEnumerable<KeyValuePair<object, object?>> values)
    {
        if (dict is IDictionary id)
        {
            ImmutableDictionary<object, object?>.Builder builder = ImmutableDictionary.CreateBuilder<object, object?>();
            builder.AddRange(GetDictionaryEntries(id));
            builder.AddRange(values);
            return DataResult.CreateSuccess<object>(builder.ToImmutable());
        }

        if (dict is null)
        {
            return DataResult.CreateSuccess<object>(ImmutableDictionary.CreateRange(values));
        }

        return DataResult.CreateError($"{nameof(MergeToMap)} called with not a dict: {dict}", Optional.Create(dict));
    }

    /// <inheritdoc/>
    public override DataResult<object> MergeToMap(object? dict, IEnumerable<KeyValuePair<string, object?>> values)
    {
        return MergeToMap(dict, values.Select(p => p.MapKey(object (k) => k)));
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<KeyValuePair<object, object?>>> GetMapValues(object? input)
    {
        if (input is not IDictionary id)
        {
            return DataResult.CreateError<IEnumerable<KeyValuePair<object, object?>>>($"{nameof(GetMapValues)} called with not a dict: {input}");
        }

        return DataResult.CreateSuccess(GetDictionaryEntries(id));
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<object?>> GetList(object? input)
    {
        if (input is not IList il)
        {
            return DataResult.CreateError<IEnumerable<object?>>($"{nameof(GetList)} called with not a list: {input}");
        }

        return DataResult.CreateSuccess(il.Cast<object?>());
    }

    /// <inheritdoc/>
    public override DataResult<MapLike<object>> GetMap(object? input)
    {
        if (input is not IDictionary id)
        {
            return DataResult.CreateError<MapLike<object>>($"{nameof(GetMap)} called with not a dict: {input}");
        }

        return DataResult.CreateSuccess(MapLike<object>.ForMap(ImmutableDictionary.CreateRange(GetDictionaryEntries(id)), this));
    }

    /// <inheritdoc/>
    public override DataResult<object> Get(object? input, object key)
    {
        ThrowIfKeyNull(key);

        if (input is not IDictionary id)
        {
            return DataResult.CreateError<object>($"{nameof(Get)} called with not a dict: {input}");
        }

        object value = id[key];
        return value is null
            ? DataResult.CreateError<object>($"No value found for {key}")
            : DataResult.CreateSuccess(value);
    }

    /// <inheritdoc/>
    public override DataResult<object> Get(object? input, string key)
    {
        ThrowIfKeyNull(key);

        return Get(input, (object) key);
    }

    /// <inheritdoc/>
    public override object? Set(object? input, string key, object? value)
    {
        return Set(input, (object) key, value);
    }

    /// <inheritdoc/>
    public override object? Update(object? input, string key, Func<object?, object?> updater)
    {
        ThrowIfKeyNull(key);

        return Update(input, (object) key, updater);
    }

    /// <inheritdoc/>
    public override object? Remove(object? input, object key)
    {
        ThrowIfKeyNull(key);

        if (input is not IDictionary id)
        {
            return input;
        }

        ImmutableDictionary<object, object>.Builder builder = ImmutableDictionary.CreateBuilder<object, object>();
        builder.AddRange(id.Cast<DictionaryEntry>().Select(e => new KeyValuePair<object, object>(e.Key, e.Value)));
        builder.Remove(key);
        return builder.ToImmutable();
    }

    /// <inheritdoc/>
    public override object? Remove(object? input, string key)
    {
        ThrowIfKeyNull(key);

        return Remove(input, (object) key);
    }

    /// <inheritdoc/>
    public override object? Copy(object? source)
    {
        return source;
    }

    /// <inheritdoc/>
    public override bool IsEmpty(object? input)
    {
        return input is null;
    }

    private TOther ConvertManaged<TOther>(DynamicOps<TOther> otherOp, object input)
        where TOther : notnull
    {
        return input switch
        {
            IDictionary => this.ConvertMap(otherOp, input),
            IList => this.ConvertList(otherOp, input),
            Stream stream => otherOp.CreateStream(stream),
            _ => throw new NotSupportedException($"Unsupported convert of {input}")
        };
    }

    private TOther ConvertNumber<TOther>(DynamicOps<TOther> otherOp, object input)
        where TOther : notnull
    {
        if (input is IConvertible c)
        {
            return otherOp.CreateDouble(c.ToDouble(null));
        }

        throw new NotSupportedException($"Unsupported primitive type: {input.GetType()}");
    }

    private static IEnumerable<KeyValuePair<object, object?>> GetDictionaryEntries(IDictionary dictionary)
    {
        // ReSharper disable once GenericEnumeratorNotDisposed
        IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return new KeyValuePair<object, object?>(enumerator.Key, enumerator.Value);
        }
    }

    private sealed class FixedDictionaryBuilder(DynamicOps<object> ops) : MapBuilderBase<object, ImmutableDictionary<object, object?>.Builder>(ops)
    {
        protected override ImmutableDictionary<object, object?>.Builder InitBuilder()
        {
            return ImmutableDictionary.CreateBuilder<object, object?>();
        }

        protected override DataResult<object> BuildResult(ImmutableDictionary<object, object?>.Builder builder, object? prefix)
        {
            return Ops.MergeToMap(prefix, builder.ToImmutable());
        }

        protected override ImmutableDictionary<object, object?>.Builder Append(object key, object? value, ImmutableDictionary<object, object?>.Builder builder)
        {
            return builder.AddAndReturn(key, value);
        }

        protected override ImmutableDictionary<object, object?>.Builder Append(string key, object? value, ImmutableDictionary<object, object?>.Builder builder)
        {
            return builder.AddAndReturn((object) key, value);
        }
    }
}