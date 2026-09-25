using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// Operations for <c>System.Text.Json</c>.
/// </summary>
public sealed class JsonOps : DynamicOps<JsonNode>
{
    /// <summary>
    /// Instance of <see cref="T:DataFixerUpper.Serialization.DynamicOps.JsonOps"/>.
    /// </summary>
    public static readonly JsonOps Instance = new(false);

    //public static readonly JsonOps Compressed = new(true);
    private JsonOps(bool compressed)
    {
        _compressed = compressed;
    }

    private readonly bool _compressed;

    /// <inheritdoc/>
    public override JsonNode? Empty()
    {
        return null;
    }

    /// <inheritdoc/>
    public override TOther? ConvertTo<TOther>(DynamicOps<TOther> otherOp, JsonNode? input)
        where TOther : default
    {
        if (IsEmpty(input))
        {
            return otherOp.Empty();
        }

        return input.GetValueKind() switch
        {
            JsonValueKind.Object => this.ConvertMap(otherOp, input),
            JsonValueKind.Array => this.ConvertList(otherOp, input),
            JsonValueKind.String => otherOp.CreateString(input.GetValue<string>()),
            JsonValueKind.Number => otherOp.CreateNumber(input.GetValue<decimal>()),
            JsonValueKind.True => otherOp.CreateBoolValue(true),
            JsonValueKind.False => otherOp.CreateBoolValue(false),
            JsonValueKind.Null => otherOp.Empty(),
            _ => throw new NotSupportedException($"{nameof(ConvertTo)} not supported for {input.GetValueKind()}")
        };
    }

    /// <inheritdoc/>
    public override DataResult<TNumber> GetNumberValue<TNumber>(JsonNode? input)
        where TNumber : default
    {
        if (input is not JsonValue jsonValue || jsonValue.GetValueKind() != JsonValueKind.Number)
        {
            return DataResult.CreateError<TNumber>($"{nameof(GetNumberValue)} called with not an number: {input}");
        }
        
        if (!decimal.TryParse(jsonValue.ToString(), out decimal num))
        {
            return DataResult.CreateError<TNumber>($"Cannot parse value of {input}");
        }

        return DataResult.CreateSuccess(TNumber.CreateSaturating(num));
    }

    /// <inheritdoc/>
    public override JsonNode CreateNumber<TNumber>(TNumber number)
        where TNumber : default
    {
        return JsonValue.Create(decimal.CreateSaturating(number));
    }

    /// <inheritdoc/>
    public override DataResult<string> GetStringValue(JsonNode? @string)
    {
        if (@string is not JsonValue jsonValue || jsonValue.GetValueKind() != JsonValueKind.String)
        {
            return DataResult.CreateError<string>($"{nameof(GetStringValue)} called with not a string: {@string}");
        }

        return DataResult.CreateSuccess(jsonValue.GetValue<string>());
    }

    /// <inheritdoc/>
    public override JsonNode CreateString(string @string)
    {
        return JsonValue.Create(@string);
    }

    /// <inheritdoc/>
    public override DataResult<bool> GetBoolValue(JsonNode? @bool)
    {
        return @bool?.GetValueKind() switch
        {
            JsonValueKind.True => DataResult.CreateSuccess(true),
            JsonValueKind.False => DataResult.CreateSuccess(false),
            _ => DataResult.CreateError<bool>($"{nameof(GetBoolValue)} called with not a boolean: {@bool}")
        };
    }

    /// <inheritdoc/>
    public override JsonNode CreateBoolValue(bool @bool)
    {
        return JsonValue.Create(@bool);
    }

    /// <inheritdoc/>
    public override JsonNode CreateList(IEnumerable<JsonNode?> list)
    {
        return list.Select(v => v?.DeepClone()).Aggregate(new JsonArray(), (arr, value) => arr.AddAndReturn(value));
    }

    /// <inheritdoc/>
    public override IListBuilder<JsonNode> CreateListBuilder()
    {
        return new JsonArrayBuilder(this);
    }

    /// <inheritdoc/>
    public override JsonNode CreateMap(IEnumerable<KeyValuePair<string, JsonNode?>> entries)
    {
        return new JsonObject(entries.Select(pair => pair.MapValue(v => v?.DeepClone())));
    }

    /// <inheritdoc/>
    public override JsonNode CreateMap(IEnumerable<KeyValuePair<JsonNode, JsonNode?>> entries)
    {
        JsonObject jsonObject = new();
        foreach ((JsonNode key, JsonNode? value) in entries)
        {
            if (key.GetValueKind() != JsonValueKind.String)
            {
                continue;
            }

            jsonObject.Add(key.GetValue<string>(), value?.DeepClone());
        }

        return jsonObject;
    }

    /// <inheritdoc/>
    public override IRecordBuilder<JsonNode> CreateMapBuilder()
    {
        return new JsonMapBuilder(this);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToList(JsonNode? list, JsonNode? other)
    {
        if (list is not JsonArray && !IsEmpty(list))
        {
            return DataResult.CreateError($"{nameof(MergeToList)} called with not a list: {list}", Optional.Create(list));
        }

        JsonArray newArray = new JsonArray().AddAndReturn(other?.DeepClone());
        if (!IsEmpty(list))
        {
            newArray.AddAll(list.AsArray().Select(v => v?.DeepClone()));
        }

        return DataResult.CreateSuccess<JsonNode>(newArray);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToList(JsonNode? list, IEnumerable<JsonNode?> values)
    {
        if (list is not JsonArray && !IsEmpty(list))
        {
            return DataResult.CreateError($"{nameof(MergeToList)} called with not a list: {list}", Optional.Create(list));
        }

        IEnumerable<JsonNode?> pending = values;
        if (!IsEmpty(list))
        {
            pending = pending.Concat(list.AsArray());
        }

        JsonArray newArray = new JsonArray().AddAll(pending.Select(v => v?.DeepClone()));
        return DataResult.CreateSuccess<JsonNode>(newArray);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToMap(JsonNode? map, string key, JsonNode? value)
    {
        if (map is not JsonObject && !IsEmpty(map))
        {
            return DataResult.CreateError($"{nameof(MergeToMap)} called with not a map: {map}", Optional.Create(map));
        }

        JsonObject newObject = IsEmpty(map) ? new JsonObject() : map.DeepClone().AsObject();
        newObject[key] = value?.DeepClone();
        return DataResult.CreateSuccess<JsonNode>(newObject);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToMap(JsonNode? map, JsonNode key, JsonNode? value)
    {
        if (key.GetValueKind() != JsonValueKind.String)
        {
            return DataResult.CreateError($"Key is not a string: {key}", Optional.Create(map));
        }

        return MergeToMap(map, key.GetValue<string>(), value);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToMap(JsonNode? map, IEnumerable<KeyValuePair<string, JsonNode?>> values)
    {
        if (map is not JsonObject && !IsEmpty(map))
        {
            return DataResult.CreateError($"{nameof(MergeToMap)} called with not a map: {map}", Optional.Create(map));
        }

        JsonObject newObject = IsEmpty(map) ? new JsonObject() : map.DeepClone().AsObject();
        JsonObject result = values.Aggregate(
            newObject, (obj, pair) =>
            {
                obj[pair.Key] = pair.Value?.DeepClone();
                return obj;
            }
        );
        return DataResult.CreateSuccess<JsonNode>(result);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> MergeToMap(JsonNode? map, IEnumerable<KeyValuePair<JsonNode, JsonNode?>> values)
    {
        if (map is not JsonObject && !IsEmpty(map))
        {
            return DataResult.CreateError($"{nameof(MergeToMap)} called with not a map: {map}", Optional.Create(map));
        }


        JsonObject newObject = IsEmpty(map) ? new JsonObject() : map.DeepClone().AsObject();
        LinkedList<JsonNode> fails = new();

        foreach ((JsonNode key, JsonNode? value) in values)
        {
            if (key.GetValueKind() != JsonValueKind.String)
            {
                fails.AddLast(key);
                continue;
            }

            newObject[key.GetValue<string>()] = value?.DeepClone();
        }

        return fails.Count > 0
            ? DataResult.CreateError($"Some keys are not strings: {CreateList(fails)}", Optional.Create<JsonNode>(newObject))
            : DataResult.CreateSuccess<JsonNode>(newObject);
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<KeyValuePair<JsonNode, JsonNode?>>> GetMapValues(JsonNode? input)
    {
        if (input is not JsonObject jsonObject)
        {
            return DataResult.CreateError<IEnumerable<KeyValuePair<JsonNode, JsonNode?>>>($"{nameof(GetMapValues)} called with not a map: {input}");
        }

        return DataResult.CreateSuccess(jsonObject.Select(pair => pair.MapKey(CreateString)));
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<JsonNode?>> GetListValues(JsonNode? input)
    {
        if (input is not JsonArray jsonArray)
        {
            return DataResult.CreateError<IEnumerable<JsonNode?>>($"{nameof(GetListValues)} called with not a list: {input}");
        }

        return DataResult.CreateSuccess<IEnumerable<JsonNode?>>(jsonArray);
    }

    /// <inheritdoc/>
    public override DataResult<IMapLike<JsonNode>> GetMap(JsonNode? input)
    {
        if (input is not JsonObject jsonObject)
        {
            return DataResult.CreateError<IMapLike<JsonNode>>($"{nameof(GetMap)} called with not a map: {input}");
        }

        return DataResult.CreateSuccess<IMapLike<JsonNode>>(new JsonMap(jsonObject, this));
    }

    /// <inheritdoc/>
    public override DataResult<ImmutableList<JsonNode?>> GetList(JsonNode? input)
    {
        if (input is not JsonArray jsonArray)
        {
            return DataResult.CreateError<ImmutableList<JsonNode?>>($"{nameof(GetList)} called with not a list: {input}");
        }

        return DataResult.CreateSuccess(ImmutableList.CreateRange(jsonArray));
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> Get(JsonNode? input, string key)
    {
        if (input is not JsonObject jsonObject)
        {
            return DataResult.CreateError<JsonNode>($"{nameof(Get)} called with not a map: {input}");
        }

        JsonNode? value = jsonObject[key];
        return value is null
            ? DataResult.CreateError<JsonNode>($"No value found for {key}")
            : DataResult.CreateSuccess(value);
    }

    /// <inheritdoc/>
    public override DataResult<JsonNode> Get(JsonNode? input, JsonNode key)
    {
        if (key.GetValueKind() != JsonValueKind.String)
        {
            return DataResult.CreateError<JsonNode>($"Key is not a string: {key}");
        }

        return Get(input, key.GetValue<string>());
    }

    /// <inheritdoc/>
    public override JsonNode? Remove(JsonNode? input, string key)
    {
        if (input is not JsonObject jsonObject)
        {
            return input;
        }

        JsonObject result = new(jsonObject);
        result.Remove(key);
        return result;
    }

    /// <inheritdoc/>
    public override JsonNode? Remove(JsonNode? input, JsonNode key)
    {
        if (key.GetValueKind() != JsonValueKind.String)
        {
            return input;
        }

        return Remove(input, key.GetValue<string>());
    }

    /// <inheritdoc/>
    public override bool IsEmpty([NotNullWhen(false)] JsonNode? input)
    {
        return input is null;
    }

    /// <inheritdoc/>
    public override bool CompressMaps()
    {
        return _compressed;
    }

    /// <inheritdoc/>
    public override JsonNode? Copy(JsonNode? source)
    {
        return source?.DeepClone();
    }

    private sealed class JsonMap(JsonObject jsonObject, JsonOps ops) : IMapLike<JsonNode>
    {
        public int Count => jsonObject.Count;
        public JsonNode? this[JsonNode key] => key.GetValueKind() != JsonValueKind.String ? null : jsonObject[key.GetValue<string>()];

        public JsonNode? this[string key] => jsonObject[key];

        public IEnumerator<KeyValuePair<JsonNode, JsonNode?>> GetEnumerator()
        {
            return jsonObject.Select(pair => pair.MapKey(ops.CreateString)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private sealed class JsonMapBuilder(JsonOps ops) : MapBuilderBase<JsonNode, JsonObject>(ops)
    {
        protected override JsonObject InitBuilder()
        {
            return new JsonObject();
        }

        protected override JsonObject Append(JsonNode key, JsonNode? value, JsonObject builder)
        {
            if (key.GetValueKind() != JsonValueKind.String)
            {
                return builder;
            }

            return Append(key.GetValue<string>(), value, builder);
        }

        protected override JsonObject Append(string key, JsonNode? value, JsonObject builder)
        {
            builder[key] = value?.DeepClone();
            return builder;
        }

        protected override DataResult<JsonNode> BuildResult(JsonObject builder, JsonNode? prefix)
        {
            if (Ops.IsEmpty(prefix))
            {
                return DataResult.CreateSuccess<JsonNode>(builder);
            }

            if (prefix is not JsonObject prefixObject)
            {
                return DataResult.CreateError($"Cannot merge json object into not an object: {prefix}", Optional.Create(prefix));
            }

            JsonObject merged = prefixObject.Aggregate(
                builder, (obj, pair) =>
                {
                    obj[pair.Key] = pair.Value;
                    return obj;
                }
            );

            return DataResult.CreateSuccess<JsonNode>(merged);
        }
    }

    private sealed class JsonArrayBuilder(JsonOps ops) : IListBuilder<JsonNode>
    {
        public DynamicOps<JsonNode> Ops => ops;
        private DataResult<JsonArray> _builder = DataResult.CreateSuccess(new JsonArray(), Lifecycle.Stable);

        public IListBuilder<JsonNode> Add(JsonNode? value)
        {
            _builder = _builder.Map(builder => builder.AddAndReturn(value?.DeepClone()));
            return this;
        }

        public IListBuilder<JsonNode> Add(DataResult<JsonNode> value)
        {
            #nullable disable
            _builder = _builder.CombineStable(Functions.AddToFirst, value.Map(v => v.DeepClone()));
            return this;
            #nullable restore
        }

        public IListBuilder<JsonNode> WithErrorsFrom<TOther>(DataResult<TOther> result)
        {
            _builder = _builder.FlatMap(array => result.Map(_ => array));
            return this;
        }

        public IListBuilder<JsonNode> MapError(UnaryOperation<string> mapper)
        {
            _builder = _builder.MapError(mapper);
            return this;
        }

        public DataResult<JsonNode> Build(JsonNode? prefix)
        {
            DataResult<JsonNode> result = _builder.FlatMap(jsonArray =>
                {
                    if (Ops.IsEmpty(prefix))
                    {
                        return DataResult.CreateSuccess<JsonNode>(jsonArray);
                    }

                    if (prefix is not JsonArray prefixArray)
                    {
                        return DataResult.CreateError($"Cannot append json array to not an array: {prefix}", Optional.Create(prefix));
                    }

                    JsonArray merged = prefixArray.AddAll(jsonArray);

                    return DataResult.CreateSuccess<JsonNode>(merged, Lifecycle.Stable);
                }
            );

            _builder = DataResult.CreateSuccess(new JsonArray(), Lifecycle.Stable);
            return result;
        }
    }
}

[JsonSerializable(typeof(decimal))]
internal partial class JsonOpsContext : JsonSerializerContext;