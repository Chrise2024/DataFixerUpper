using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

public static partial class Codec
{
    /// <summary>
    /// Codec of <see langword="byte"/>.
    /// </summary>
    public static NumberCodec<byte> Byte => new();
    
    /// <summary>
    /// Codec of list of <see langword="byte"/>.
    /// </summary>
    public static NumberListValuesCodec<byte> ByteList => new();
    
    /// <summary>
    /// Codec of <see langword="short"/>.
    /// </summary>
    public static NumberCodec<short> Short => new();
    
    /// <summary>
    /// Codec of list of <see langword="short"/>.
    /// </summary>
    public static NumberListValuesCodec<short> ShortList => new();
    
    /// <summary>
    /// Codec of <see langword="int"/>.
    /// </summary>
    public static NumberCodec<int> Int => new();
    
    /// <summary>
    /// Codec of list of <see langword="int"/>.
    /// </summary>
    public static NumberListValuesCodec<int> IntList => new();
    
    /// <summary>
    /// Codec of <see langword="long"/>.
    /// </summary>
    public static NumberCodec<long> Long => new();
    
    /// <summary>
    /// Codec of list of <see langword="long"/>.
    /// </summary>
    public static NumberListValuesCodec<long> LongList => new();
    
    /// <summary>
    /// Codec of <see langword="float"/>.
    /// </summary>
    public static NumberCodec<float> Float => new();
    
    /// <summary>
    /// Codec of list of <see langword="float"/>.
    /// </summary>
    public static NumberListValuesCodec<float> FloatList => new();
    
    /// <summary>
    /// Codec of <see langword="double"/>.
    /// </summary>
    public static NumberCodec<double> Double => new();
    
    /// <summary>
    /// Codec of list of <see langword="double"/>.
    /// </summary>
    public static NumberListValuesCodec<double> DoubleList => new();
    
    /// <summary>
    /// Codec of <see langword="decimal"/>.
    /// </summary>
    public static NumberCodec<decimal> Decimal => new();
    
    /// <summary>
    /// Codec of list of <see langword="decimal"/>.
    /// </summary>
    public static NumberListValuesCodec<decimal> DecimalList => new();
    
    /// <summary>
    /// Codec of <see langword="bool"/>.
    /// </summary>
    public static BooleanCodec Bool => new();
    
    /// <summary>
    /// Codec of <see langword="string"/>.
    /// </summary>
    public static StringCodec String => new();
    
    /// <summary>
    /// Codec of <see langword="byte"/> buffer.
    /// </summary>
    public static StreamCodec Stream => new();

    /// <summary>
    /// Codec that cast input into <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> type and passes serialized input while decoding.
    /// </summary>
    public static Codec<IDynamic> PassThrough => new PassThroughCodec();

    /// <summary>
    /// Empty codec.
    /// </summary>
    public static MapCodec<Unit> Empty => MapCodec.CreateUnit(Unit.Instance);
}