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
    
    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return _encoderDispatcher.Apply(input).Encode(input, ops, prefix);
    }
    
    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return _decoder.Decode(ops, input);
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
    
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _encoder.GetKeys(ops);
    }
    
    public override IRecordBuilder<TObject> Encode<TObject>(T2 input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return _encoder.Encode(_getter.Apply(i), ops, prefix);
    }
    
    public override string ToString()
    {
        return $"{_encoder}[Mapped]";
    }
}

internal sealed class DependentRecordDecoder<TElement, TField>(
    IMapEncoder<TElement> encoder,
    IMapDecoder<TField> decoder,
    Func<TField, IMapDecoder<TElement>> dispatcher
) : MapDecoderBase<TElement>
{    
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return encoder.GetKeys(ops);
    }
    
    public override DataResult<TElement> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return decoder.Decode(ops, input).Map(dispatcher).FlatMap(eDecoder => eDecoder.Decode(ops, input));
    }
    
    public override string ToString()
    {
        return $"Dependent[{encoder}]";
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
    
    public override IRecordBuilder<TObject> Encode<TObject>(T2 input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode(_ => input, ops, prefix);
        _encoder.Encode(_fromInstance, ops, prefix);
        return prefix;
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
    
    public override DataResult<T2> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return _decoder.Decode(ops, input).FlatMap(t1 => _funcDecoder.Decode(ops, input).Map(f => f.Apply(t1)));
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
    
    public override IRecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        return prefix;
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
    
    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return DataResult.Unbox(
            DataResultOperator.Instance.Combine(
                _funcDecoder.Decode(ops, input),
                _decoder1.Decode(ops, input),
                _decoder2.Decode(ops, input)
            )
        );
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
    
    public override IRecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        _encoder3.Encode(_fromInstance3, ops, prefix);
        return prefix;
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
    
    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
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
    
    public override IRecordBuilder<TObject> Encode<TObject>(TR input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        _funcEncoder.Encode((_, _, _, _) => input, ops, prefix);
        _encoder1.Encode(_fromInstance1, ops, prefix);
        _encoder2.Encode(_fromInstance2, ops, prefix);
        _encoder3.Encode(_fromInstance3, ops, prefix);
        _encoder4.Encode(_fromInstance4, ops, prefix);
        return prefix;
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
    
    public override DataResult<TR> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
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
}