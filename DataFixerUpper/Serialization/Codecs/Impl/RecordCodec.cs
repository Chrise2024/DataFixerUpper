using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs.Builder;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class RecordCodec<T>(RecordCodecBuilder<T, T> builder) : MapCodec<T>
{
    private readonly Func<T, IMapEncoder<T>> _encoderDispatcher = builder.EncoderDispatcher;
    private readonly IMapDecoder<T> _decoder = builder.Decoder;

    public override ValueHolder<string> CodecNameHolder => $"RecordCodec[{_decoder}]";

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _decoder.GetKeys(ops);
    }

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _encoderDispatcher.Apply(input).Encode(input, ops, prefix);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _decoder.Decode(ops, input);
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordCodec<T> codec && _encoderDispatcher.Equals(codec._encoderDispatcher) && _decoder.Equals(codec._decoder);
    }

    public override int GetHashCode()
    {
        return _encoderDispatcher.GetHashCode() + _decoder.GetHashCode() * 31;
    }
}

internal sealed class MappedRecordEncoder<TInstance, T1, T2>(
    RecordCodecBuilder<TInstance, T1> builder,
    TInstance i
) : MapEncoderBase<T2>
{
    internal static Func<TInstance, MappedRecordEncoder<TInstance, T1, T2>> Create(RecordCodecBuilder<TInstance, T1> builder)
    {
        return i => new MappedRecordEncoder<TInstance, T1, T2>(builder, i);
    }

    private readonly Func<TInstance, T1> _getter = builder.Getter;
    private readonly IMapEncoder<T1> _encoder = builder.EncoderDispatcher.Apply(i);
    private readonly TInstance _i = i;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _encoder.GetKeys(ops);
    }

    public override RecordBuilder<TObject> Encode<TObject>(T2 input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _encoder.Encode(_getter.Apply(_i), ops, prefix);
    }

    public override string ToString()
    {
        return $"{_encoder}[Mapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is MappedRecordEncoder<TInstance, T1, T2> encoder
            && _getter.Equals(encoder._getter)
            && _encoder.Equals(encoder._encoder)
            && EqualityComparer<TInstance>.Default.Equals(_i, encoder._i);
    }

    public override int GetHashCode()
    {
        int hash = _getter.GetHashCode();
        hash = hash * 31 + _encoder.GetHashCode();
        hash = hash * 31 + (_i is null ? 0 : _i.GetHashCode());
        return hash;
    }
}

internal sealed class DependentRecordDecoder<TElement, TField>(
    IMapEncoder<TElement> encoder,
    IMapDecoder<TField> decoder,
    Func<TField, IMapDecoder<TElement>> dispatcher
) : MapDecoderBase<TElement>
{
    private readonly IMapEncoder<TElement> _encoder = encoder;

    private readonly IMapDecoder<TField> _decoder = decoder;

    private readonly Func<TField, IMapDecoder<TElement>> _dispatcher = dispatcher;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _encoder.GetKeys(ops);
    }

    public override DataResult<TElement> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _decoder.Decode(ops, input).Map(_dispatcher).FlatMap(eDecoder => eDecoder.Decode(ops, input));
    }

    public override string ToString()
    {
        return $"Dependent[{_encoder}]";
    }

    public override bool Equals(object? obj)
    {
        return obj is DependentRecordDecoder<TElement, TField> decoder
            && _encoder.Equals(decoder._encoder)
            && _decoder.Equals(decoder._decoder)
            && _dispatcher.Equals(decoder._dispatcher);
    }

    public override int GetHashCode()
    {
        int hash = _encoder.GetHashCode();
        hash = hash * 31 + _decoder.GetHashCode();
        hash = hash * 31 + _dispatcher.GetHashCode();
        return hash;
    }
}

internal sealed class LiftedRecordEncoder<TInstance, T1, T2>(
    RecordCodecBuilder<TInstance, Func<T1, T2>> func,
    RecordCodecBuilder<TInstance, T1> builder,
    TInstance i
) : MapEncoderBase<T2>
{
    internal static Func<TInstance, LiftedRecordEncoder<TInstance, T1, T2>> Create(
        RecordCodecBuilder<TInstance, Func<T1, T2>> func,
        RecordCodecBuilder<TInstance, T1> builder
    )
    {
        return i => new LiftedRecordEncoder<TInstance, T1, T2>(func, builder, i);
    }

    private readonly IMapEncoder<Func<T1, T2>> _funcEncoder = func.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T1> _encoder = builder.EncoderDispatcher.Apply(i);
    private readonly T1 _fromInstance = builder.Getter.Apply(i);

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcEncoder.GetKeys(ops)
            .Concat(_encoder.GetKeys(ops));
    }

    public override RecordBuilder<TObject> Encode<TObject>(T2 input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode(_ => input, ops, prefix);
        _encoder.Encode(_fromInstance, ops, prefix);
        return prefix;
    }

    public override bool Equals(object? obj)
    {
        return obj is LiftedRecordEncoder<TInstance, T1, T2> encoder
            && _funcEncoder.Equals(encoder._funcEncoder)
            && _encoder.Equals(encoder._encoder)
            && EqualityComparer<T1>.Default.Equals(_fromInstance, encoder._fromInstance);
    }

    public override int GetHashCode()
    {
        int hash = _funcEncoder.GetHashCode();
        hash = hash * 31 + _encoder.GetHashCode();
        hash = hash * 31 + (_fromInstance is null ? 0 : _fromInstance.GetHashCode());
        return hash;
    }
}

internal sealed class LiftedRecordDecoder<TInstance, T1, T2>(
    RecordCodecBuilder<TInstance, Func<T1, T2>> func,
    RecordCodecBuilder<TInstance, T1> builder
) : MapDecoderBase<T2>
{
    private readonly IMapDecoder<Func<T1, T2>> _funcDecoder = func.Decoder;
    private readonly IMapDecoder<T1> _decoder = builder.Decoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcDecoder.GetKeys(ops)
            .Concat(_decoder.GetKeys(ops));
    }

    public override DataResult<T2> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _decoder.Decode(ops, input).FlatMap(t1 => _funcDecoder.Decode(ops, input).Map(f => f.Apply(t1)));
    }

    public override bool Equals(object? obj)
    {
        return obj is LiftedRecordDecoder<TInstance, T1, T2> decoder
            && _funcDecoder.Equals(decoder._funcDecoder)
            && _decoder.Equals(decoder._decoder);
    }

    public override int GetHashCode()
    {
        return _funcDecoder.GetHashCode() + _decoder.GetHashCode() * 31;
    }
}

internal sealed class RecordEncoder2<TInstance, T1, T2, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2,
    TInstance i
) : MapEncoderBase<TR>
{
    internal static Func<TInstance, RecordEncoder2<TInstance, T1, T2, TR>> Create(
        RecordCodecBuilder<TInstance, Func<T1, T2, TR>> func,
        RecordCodecBuilder<TInstance, T1> f1,
        RecordCodecBuilder<TInstance, T2> f2
    )
    {
        return i => new RecordEncoder2<TInstance, T1, T2, TR>(func, f1, f2, i);
    }

    private readonly IMapEncoder<Func<T1, T2, TR>> _funcEncoder = func.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T1> _encoder1 = f1.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T2> _encoder2 = f2.EncoderDispatcher.Apply(i);
    private readonly T1 _fromInstance1 = f1.Getter.Apply(i);
    private readonly T2 _fromInstance2 = f2.Getter.Apply(i);

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcEncoder.GetKeys(ops)
            .Concat(_encoder1.GetKeys(ops))
            .Concat(_encoder2.GetKeys(ops));
    }

    public override RecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        return prefix;
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordEncoder2<TInstance, T1, T2, TR> encoder
            && _funcEncoder.Equals(encoder._funcEncoder)
            && _encoder1.Equals(encoder._encoder1)
            && _encoder2.Equals(encoder._encoder2)
            && EqualityComparer<T1>.Default.Equals(_fromInstance1, encoder._fromInstance1)
            && EqualityComparer<T2>.Default.Equals(_fromInstance2, encoder._fromInstance2);
    }

    public override int GetHashCode()
    {
        int hash = _funcEncoder.GetHashCode();
        hash = hash * 31 + _encoder1.GetHashCode();
        hash = hash * 31 + _encoder2.GetHashCode();
        hash = hash * 31 + (_fromInstance1 is null ? 0 : _fromInstance1.GetHashCode());
        hash = hash * 31 + (_fromInstance2 is null ? 0 : _fromInstance2.GetHashCode());
        return hash;
    }
}

internal sealed class RecordDecoder2<TInstance, T1, T2, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2
) : MapDecoderBase<TR>
{
    private readonly IMapDecoder<Func<T1, T2, TR>> _funcDecoder = func.Decoder;
    private readonly IMapDecoder<T1> _decoder1 = f1.Decoder;
    private readonly IMapDecoder<T2> _decoder2 = f2.Decoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcDecoder.GetKeys(ops)
            .Concat(_decoder1.GetKeys(ops))
            .Concat(_decoder2.GetKeys(ops));
    }

    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return DataResult.Unbox(
            DataResultOperator.Instance.Combine(
                _funcDecoder.Decode(ops, input),
                _decoder1.Decode(ops, input),
                _decoder2.Decode(ops, input)
            )
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordDecoder2<TInstance, T1, T2, TR> decoder
            && _funcDecoder.Equals(decoder._funcDecoder)
            && _decoder1.Equals(decoder._decoder1)
            && _decoder2.Equals(decoder._decoder2);
    }

    public override int GetHashCode()
    {
        int hash = _funcDecoder.GetHashCode();
        hash = hash * 31 + _decoder1.GetHashCode();
        hash = hash * 31 + _decoder2.GetHashCode();
        return hash;
    }
}

internal sealed class RecordEncoder3<TInstance, T1, T2, T3, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, T3, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2,
    RecordCodecBuilder<TInstance, T3> f3,
    TInstance i
) : MapEncoderBase<TR>
{
    internal static Func<TInstance, RecordEncoder3<TInstance, T1, T2, T3, TR>> Create(
        RecordCodecBuilder<TInstance, Func<T1, T2, T3, TR>> func,
        RecordCodecBuilder<TInstance, T1> f1,
        RecordCodecBuilder<TInstance, T2> f2,
        RecordCodecBuilder<TInstance, T3> f3
    )
    {
        return i => new RecordEncoder3<TInstance, T1, T2, T3, TR>(func, f1, f2, f3, i);
    }

    private readonly IMapEncoder<Func<T1, T2, T3, TR>> _funcEncoder = func.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T1> _encoder1 = f1.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T2> _encoder2 = f2.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T3> _encoder3 = f3.EncoderDispatcher.Apply(i);
    private readonly T1 _fromInstance1 = f1.Getter.Apply(i);
    private readonly T2 _fromInstance2 = f2.Getter.Apply(i);
    private readonly T3 _fromInstance3 = f3.Getter.Apply(i);

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcEncoder.GetKeys(ops)
            .Concat(_encoder1.GetKeys(ops))
            .Concat(_encoder2.GetKeys(ops))
            .Concat(_encoder3.GetKeys(ops));
    }

    public override RecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        _encoder3.Encode(_fromInstance3, ops, prefix);
        return prefix;
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordEncoder3<TInstance, T1, T2, T3, TR> encoder
            && _funcEncoder.Equals(encoder._funcEncoder)
            && _encoder1.Equals(encoder._encoder1)
            && _encoder2.Equals(encoder._encoder2)
            && _encoder3.Equals(encoder._encoder3)
            && EqualityComparer<T1>.Default.Equals(_fromInstance1, encoder._fromInstance1)
            && EqualityComparer<T2>.Default.Equals(_fromInstance2, encoder._fromInstance2)
            && EqualityComparer<T3>.Default.Equals(_fromInstance3, encoder._fromInstance3);
    }

    public override int GetHashCode()
    {
        int hash = _funcEncoder.GetHashCode();
        hash = hash * 31 + _encoder1.GetHashCode();
        hash = hash * 31 + _encoder2.GetHashCode();
        hash = hash * 31 + _encoder3.GetHashCode();
        hash = hash * 31 + (_fromInstance1 is null ? 0 : _fromInstance1.GetHashCode());
        hash = hash * 31 + (_fromInstance2 is null ? 0 : _fromInstance2.GetHashCode());
        hash = hash * 31 + (_fromInstance3 is null ? 0 : _fromInstance3.GetHashCode());
        return hash;
    }
}

internal sealed class RecordDecoder3<TInstance, T1, T2, T3, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, T3, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2,
    RecordCodecBuilder<TInstance, T3> f3
) : MapDecoderBase<TR>
{
    private readonly IMapDecoder<Func<T1, T2, T3, TR>> _funcDecoder = func.Decoder;
    private readonly IMapDecoder<T1> _decoder1 = f1.Decoder;
    private readonly IMapDecoder<T2> _decoder2 = f2.Decoder;
    private readonly IMapDecoder<T3> _decoder3 = f3.Decoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcDecoder.GetKeys(ops)
            .Concat(_decoder1.GetKeys(ops))
            .Concat(_decoder2.GetKeys(ops))
            .Concat(_decoder3.GetKeys(ops));
    }

    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return DataResult.Unbox(
            DataResultOperator.Instance.Combine(
                _funcDecoder.Decode(ops, input),
                _decoder1.Decode(ops, input),
                _decoder2.Decode(ops, input),
                _decoder3.Decode(ops, input)
            )
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordDecoder3<TInstance, T1, T2, T3, TR> decoder
            && _funcDecoder.Equals(decoder._funcDecoder)
            && _decoder1.Equals(decoder._decoder1)
            && _decoder2.Equals(decoder._decoder2)
            && _decoder3.Equals(decoder._decoder3);
    }

    public override int GetHashCode()
    {
        int hash = _funcDecoder.GetHashCode();
        hash = hash * 31 + _decoder1.GetHashCode();
        hash = hash * 31 + _decoder2.GetHashCode();
        hash = hash * 31 + _decoder3.GetHashCode();
        return hash;
    }
}

internal sealed class RecordEncoder4<TInstance, T1, T2, T3, T4, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, T3, T4, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2,
    RecordCodecBuilder<TInstance, T3> f3,
    RecordCodecBuilder<TInstance, T4> f4,
    TInstance i
) : MapEncoderBase<TR>
{
    internal static Func<TInstance, RecordEncoder4<TInstance, T1, T2, T3, T4, TR>> Create(
        RecordCodecBuilder<TInstance, Func<T1, T2, T3, T4, TR>> func,
        RecordCodecBuilder<TInstance, T1> f1,
        RecordCodecBuilder<TInstance, T2> f2,
        RecordCodecBuilder<TInstance, T3> f3,
        RecordCodecBuilder<TInstance, T4> f4
    )
    {
        return i => new RecordEncoder4<TInstance, T1, T2, T3, T4, TR>(func, f1, f2, f3, f4, i);
    }

    private readonly IMapEncoder<Func<T1, T2, T3, T4, TR>> _funcEncoder = func.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T1> _encoder1 = f1.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T2> _encoder2 = f2.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T3> _encoder3 = f3.EncoderDispatcher.Apply(i);
    private readonly IMapEncoder<T4> _encoder4 = f4.EncoderDispatcher.Apply(i);
    private readonly T1 _fromInstance1 = f1.Getter.Apply(i);
    private readonly T2 _fromInstance2 = f2.Getter.Apply(i);
    private readonly T3 _fromInstance3 = f3.Getter.Apply(i);
    private readonly T4 _fromInstance4 = f4.Getter.Apply(i);

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcEncoder.GetKeys(ops)
            .Concat(_encoder1.GetKeys(ops))
            .Concat(_encoder2.GetKeys(ops))
            .Concat(_encoder3.GetKeys(ops))
            .Concat(_encoder4.GetKeys(ops));
    }

    public override RecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _, _, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        _encoder3.Encode(_fromInstance3, ops, prefix);
        _encoder4.Encode(_fromInstance4, ops, prefix);
        return prefix;
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordEncoder4<TInstance, T1, T2, T3, T4, TR> encoder
            && _funcEncoder.Equals(encoder._funcEncoder)
            && _encoder1.Equals(encoder._encoder1)
            && _encoder2.Equals(encoder._encoder2)
            && _encoder3.Equals(encoder._encoder3)
            && _encoder4.Equals(encoder._encoder4)
            && EqualityComparer<T1>.Default.Equals(_fromInstance1, encoder._fromInstance1)
            && EqualityComparer<T2>.Default.Equals(_fromInstance2, encoder._fromInstance2)
            && EqualityComparer<T3>.Default.Equals(_fromInstance3, encoder._fromInstance3)
            && EqualityComparer<T4>.Default.Equals(_fromInstance4, encoder._fromInstance4);
    }

    public override int GetHashCode()
    {
        int hash = _funcEncoder.GetHashCode();
        hash = hash * 31 + _encoder1.GetHashCode();
        hash = hash * 31 + _encoder2.GetHashCode();
        hash = hash * 31 + _encoder3.GetHashCode();
        hash = hash * 31 + _encoder4.GetHashCode();
        hash = hash * 31 + (_fromInstance1 is null ? 0 : _fromInstance1.GetHashCode());
        hash = hash * 31 + (_fromInstance2 is null ? 0 : _fromInstance2.GetHashCode());
        hash = hash * 31 + (_fromInstance3 is null ? 0 : _fromInstance3.GetHashCode());
        hash = hash * 31 + (_fromInstance4 is null ? 0 : _fromInstance4.GetHashCode());
        return hash;
    }
}

internal sealed class RecordDecoder4<TInstance, T1, T2, T3, T4, TR>(
    RecordCodecBuilder<TInstance, Func<T1, T2, T3, T4, TR>> func,
    RecordCodecBuilder<TInstance, T1> f1,
    RecordCodecBuilder<TInstance, T2> f2,
    RecordCodecBuilder<TInstance, T3> f3,
    RecordCodecBuilder<TInstance, T4> f4
) : MapDecoderBase<TR>
{
    private readonly IMapDecoder<Func<T1, T2, T3, T4, TR>> _funcDecoder = func.Decoder;
    private readonly IMapDecoder<T1> _decoder1 = f1.Decoder;
    private readonly IMapDecoder<T2> _decoder2 = f2.Decoder;
    private readonly IMapDecoder<T3> _decoder3 = f3.Decoder;
    private readonly IMapDecoder<T4> _decoder4 = f4.Decoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _funcDecoder.GetKeys(ops)
            .Concat(_decoder1.GetKeys(ops))
            .Concat(_decoder2.GetKeys(ops))
            .Concat(_decoder3.GetKeys(ops))
            .Concat(_decoder4.GetKeys(ops));
    }

    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return DataResult.Unbox(
            DataResultOperator.Instance.Combine(
                _funcDecoder.Decode(ops, input),
                _decoder1.Decode(ops, input),
                _decoder2.Decode(ops, input),
                _decoder3.Decode(ops, input),
                _decoder4.Decode(ops, input)
            )
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is RecordDecoder4<TInstance, T1, T2, T3, T4, TR> decoder
            && _funcDecoder.Equals(decoder._funcDecoder)
            && _decoder1.Equals(decoder._decoder1)
            && _decoder2.Equals(decoder._decoder2)
            && _decoder3.Equals(decoder._decoder3)
            && _decoder4.Equals(decoder._decoder4);
    }

    public override int GetHashCode()
    {
        int hash = _funcDecoder.GetHashCode();
        hash = hash * 31 + _decoder1.GetHashCode();
        hash = hash * 31 + _decoder2.GetHashCode();
        hash = hash * 31 + _decoder3.GetHashCode();
        hash = hash * 31 + _decoder4.GetHashCode();
        return hash;
    }
}