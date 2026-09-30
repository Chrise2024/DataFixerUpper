using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

public static partial class Codec
{
    /// <summary>
    /// Codec of <see langword="byte"/>.
    /// </summary>
    public static ByteCodec Byte => new();
    
    /// <summary>
    /// Codec of <see langword="short"/>.
    /// </summary>
    public static ShortCodec Short => new();
    
    /// <summary>
    /// Codec of <see langword="int"/>.
    /// </summary>
    public static IntCodec Int => new();
    
    /// <summary>
    /// Codec of list of <see langword="int"/>.
    /// </summary>
    public static IntListCodec IntList => new();
    
    /// <summary>
    /// Codec of <see langword="long"/>.
    /// </summary>
    public static LongCodec Long => new();
    
    /// <summary>
    /// Codec of list of <see langword="long"/>.
    /// </summary>
    public static LongListCodec LongList => new();
    
    /// <summary>
    /// Codec of <see langword="float"/>.
    /// </summary>
    public static FloatCodec Float => new();
    
    /// <summary>
    /// Codec of <see langword="double"/>.
    /// </summary>
    public static DoubleCodec Double => new();
    
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