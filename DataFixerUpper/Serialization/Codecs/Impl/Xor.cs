using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class XorCodec<TL, TR>(Codec<TL> lCodec, Codec<TR> rCodec) : Codec<Either<TL, TR>>
{
    public override ValueHolder<string> CodecNameHolder => $"Xor[{lCodec} {rCodec}]";

    public override DataResult<TObject> Encode<TObject>(Either<TL, TR> input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return input.MapGet(
            l => lCodec.Encode(l, ops, prefix),
            r => rCodec.Encode(r, ops, prefix)
        );
    }

    public override DataResult<(Either<TL, TR>, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        DataResult<(Either<TL, TR>, TObject?)> lResult = lCodec.Decode(ops, input).Map<(Either<TL, TR>, TObject?)>(result => result.MapFirst(Either.CreateLeft<TL, TR>));
        DataResult<(Either<TL, TR>, TObject?)> rResult = rCodec.Decode(ops, input).Map<(Either<TL, TR>, TObject?)>(result => result.MapFirst(Either.CreateRight<TL, TR>));

        if (lResult.TryGetResultOrPartial(out (Either<TL, TR>, TObject?) lp) && rResult.TryGetResultOrPartial(out (Either<TL, TR>, TObject?) rp))
        {
            return DataResult.CreateError<(Either<TL, TR>, TObject?)>($"Both alternatives read successfully, can not pick the correct one. First: {lp} Second: {rp}");
        }

        if (lResult.HasResultOrPartial)
        {
            return lResult;
        }

        if (rResult.HasResultOrPartial)
        {
            return rResult;
        }

        return lResult.Combine(Functions.LiftSecond, rResult);
    }
}

internal sealed class XorMapCodec<TL, TR>(MapCodec<TL> lCodec, MapCodec<TR> rCodec) : MapCodec<Either<TL, TR>>
{
    public override ValueHolder<string> CodecNameHolder => $"Xor[{lCodec} {rCodec}]";

    public override IRecordBuilder<TObject> Encode<TObject>(Either<TL, TR> input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return input.MapGet(
            l => lCodec.Encode(l, ops, prefix),
            r => rCodec.Encode(r, ops, prefix)
        );
    }

    public override DataResult<Either<TL, TR>> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        DataResult<Either<TL, TR>> lResult = lCodec.Decode(ops, input).Map(Either.CreateLeft<TL, TR>);
        DataResult<Either<TL, TR>> rResult = rCodec.Decode(ops, input).Map(Either.CreateRight<TL, TR>);

        if (lResult.TryGetResult(out Either<TL, TR>? lr) && rResult.TryGetResult(out Either<TL, TR>? rr))
        {
            return DataResult.CreateError<Either<TL, TR>>($"Both alternatives read successfully, can not pick the correct one. First: {lr} Second: {rr}");
        }

        if (lResult.IsSuccess)
        {
            return lResult;
        }

        if (rResult.IsSuccess)
        {
            return rResult;
        }

        return lResult.Combine(Functions.LiftSecond, rResult);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return lCodec.GetKeys(ops).Concat(rCodec.GetKeys(ops));
    }
}