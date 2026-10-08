using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class EitherCodec<TL, TR>(Codec<TL> lCodec, Codec<TR> rCodec) : Codec<Either<TL, TR>>
{
    private readonly Codec<TL> _lCodec = lCodec;

    private readonly Codec<TR> _rCodec = rCodec;

    public override ValueHolder<string> CodecNameHolder => $"Either[{_lCodec} {_rCodec}]";

    public override DataResult<TObject> Encode<TObject>(Either<TL, TR> input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return input.MapGet(
            l => _lCodec.Encode(l, ops, prefix),
            r => _rCodec.Encode(r, ops, prefix)
        );
    }

    public override DataResult<(Either<TL, TR>, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        DataResult<(Either<TL, TR>, TObject?)> lResult = _lCodec.Decode(ops, input).Map<(Either<TL, TR>, TObject?)>(result => result.MapFirst(Either.CreateLeft<TL, TR>));
        if (lResult.IsSuccess)
        {
            return lResult;
        }

        DataResult<(Either<TL, TR>, TObject?)> rResult = _rCodec.Decode(ops, input).Map<(Either<TL, TR>, TObject?)>(result => result.MapFirst(Either.CreateRight<TL, TR>));
        if (rResult.IsSuccess)
        {
            return rResult;
        }

        if (lResult.HasResultOrPartial)
        {
            return lResult;
        }

        if (rResult.HasResultOrPartial)
        {
            return rResult;
        }

        return DataResult.CreateError<(Either<TL, TR>, TObject?)>($"Failed to parse either. First: {lResult.ErrorResult?.Message} Second: {rResult.ErrorResult?.Message}");
    }

    public override bool Equals(object? obj)
    {
        return obj is EitherCodec<TL, TR> codec && _lCodec.Equals(codec._lCodec) && _rCodec.Equals(codec._rCodec);
    }

    public override int GetHashCode()
    {
        return _lCodec.GetHashCode() + _rCodec.GetHashCode() * 31;
    }
}

internal sealed class EitherMapCodec<TL, TR>(MapCodec<TL> lCodec, MapCodec<TR> rCodec) : MapCodec<Either<TL, TR>>
{
    private readonly MapCodec<TL> _lCodec = lCodec;

    private readonly MapCodec<TR> _rCodec = rCodec;

    public override ValueHolder<string> CodecNameHolder => $"Either[{_lCodec} {_rCodec}]";

    public override RecordBuilder<TObject> Encode<TObject>(Either<TL, TR> input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return input.MapGet(
            l => _lCodec.Encode(l, ops, prefix),
            r => _rCodec.Encode(r, ops, prefix)
        );
    }

    public override DataResult<Either<TL, TR>> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        DataResult<Either<TL, TR>> lResult = _lCodec.Decode(ops, input).Map(Either.CreateLeft<TL, TR>);
        if (lResult.IsSuccess)
        {
            return lResult;
        }

        DataResult<Either<TL, TR>> rResult = _rCodec.Decode(ops, input).Map(Either.CreateRight<TL, TR>);
        if (rResult.IsSuccess)
        {
            return rResult;
        }

        return lResult.Combine(Functions.LiftSecond, rResult);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _lCodec.GetKeys(ops).Concat(_rCodec.GetKeys(ops));
    }

    public override bool Equals(object? obj)
    {
        return obj is EitherMapCodec<TL, TR> codec && _lCodec.Equals(codec._lCodec) && _rCodec.Equals(codec._rCodec);
    }

    public override int GetHashCode()
    {
        return _lCodec.GetHashCode() + _rCodec.GetHashCode() * 31;
    }
}