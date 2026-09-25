using System.Collections.Immutable;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;
using Newtonsoft.Json.Linq;

namespace DataFixerUpper.Test;

#nullable disable
public sealed class NewtonJsonOps : DynamicOps<JToken>
{
    public override JToken Empty() => JValue.CreateNull();

    public override TOther ConvertTo<TOther>(DynamicOps<TOther> otherOp, JToken input)
    {
        return input.Type switch
        {
            JTokenType.Null => otherOp.Empty(),
            JTokenType.Boolean => otherOp.CreateBoolValue(input.Value<bool>()),
            JTokenType.Integer => otherOp.CreateNumber(input.Value<long>()),
            JTokenType.Float => otherOp.CreateNumber(input.Value<decimal>()),
            JTokenType.String => otherOp.CreateString(input.Value<string>()),
            JTokenType.Array => otherOp.CreateList(input.Select(i => ConvertTo(otherOp, i))),
            JTokenType.Object => otherOp.CreateMap(input.Children<JProperty>().Select(p => new KeyValuePair<string, TOther>(p.Name, ConvertTo(otherOp, p.Value)))),
            _ => throw new NotSupportedException($"Unsupported JToken type: {input.Type}")
        };
    }

    public override DataResult<TNumber> GetNumberValue<TNumber>(JToken input) where TNumber : default
    {
        return input.Type switch
        {
            JTokenType.Integer => DataResult.CreateSuccess(TNumber.CreateSaturating(input.Value<long>()), Lifecycle.Stable),
            JTokenType.Float => DataResult.CreateSuccess(TNumber.CreateSaturating(input.Value<decimal>()), Lifecycle.Stable),
            _ => DataResult.CreateError<TNumber>($"Input is not a number: {input}")
        };
    }

    public override JToken CreateNumber<TNumber>(TNumber number) where TNumber : default
    {
        return TNumber.IsInteger(number) ? new JValue(long.CreateSaturating(number)) : new JValue(decimal.CreateSaturating(number));
    }

    public override DataResult<string> GetStringValue(JToken @string)
    {
        if (@string.Type == JTokenType.String && @string.Value<string>() is { } s)
        {
            return DataResult.CreateSuccess(s, Lifecycle.Stable);
        }

        return DataResult.CreateError<string>($"Input is not a string: {@string}");
    }

    public override JToken CreateString(string @string)
    {
        return new JValue(@string);
    }

    public override DataResult<bool> GetBoolValue(JToken @bool)
    {
        if (@bool.Type == JTokenType.Boolean)
        {
            return DataResult.CreateSuccess(@bool.Value<bool>(), Lifecycle.Stable);
        }

        return DataResult.CreateError<bool>($"Input is not a boolean: {@bool}");
    }

    public override JToken CreateBoolValue(bool @bool)
    {
        return new JValue(@bool);
    }

    public override JToken CreateMap(IEnumerable<KeyValuePair<JToken, JToken>> entries)
    {
        return entries.Aggregate(
            new JObject(), (jObject, pair) =>
            {
                if (pair.Key.Type == JTokenType.String && pair.Key.Value<string>() is { } key)
                {
                    jObject[key] = pair.Value;
                }

                return jObject;
            }
        );
    }

    public override JToken CreateMap(IEnumerable<KeyValuePair<string, JToken>> entries)
    {
        return entries.Aggregate(new JObject(), Functions.AddToFirst);
    }

    public override JToken CreateList(IEnumerable<JToken> list)
    {
        return list.Aggregate(new JArray(), Functions.AddToFirst);
    }

    public override DataResult<JToken> MergeToList(JToken list, JToken other)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> MergeToList(JToken list, IEnumerable<JToken> values)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> MergeToMap(JToken map, JToken key, JToken value)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> MergeToMap(JToken map, string key, JToken value)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> MergeToMap(JToken map, IEnumerable<KeyValuePair<string, JToken>> values)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> MergeToMap(JToken map, IEnumerable<KeyValuePair<JToken, JToken>> values)
    {
        throw new NotImplementedException();
    }

    public override DataResult<IEnumerable<KeyValuePair<JToken, JToken>>> GetMapValues(JToken input)
    {
        throw new NotImplementedException();
    }

    public override DataResult<IEnumerable<JToken>> GetListValues(JToken input)
    {
        throw new NotImplementedException();
    }

    public override DataResult<IMapLike<JToken>> GetMap(JToken input)
    {
        throw new NotImplementedException();
    }

    public override DataResult<ImmutableList<JToken>> GetList(JToken input)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> Get(JToken input, JToken key)
    {
        throw new NotImplementedException();
    }

    public override DataResult<JToken> Get(JToken input, string key)
    {
        throw new NotImplementedException();
    }

    public override JToken Remove(JToken input, JToken key)
    {
        throw new NotImplementedException();
    }

    public override JToken Remove(JToken input, string key)
    {
        throw new NotImplementedException();
    }

    public override JToken Copy(JToken source)
    {
        return source.DeepClone();
    }

    public override bool IsEmpty(JToken input)
    {
        return JToken.EqualityComparer.Equals(input, JsonNull);
    }

    private static readonly JToken JsonNull = JValue.CreateNull();
}