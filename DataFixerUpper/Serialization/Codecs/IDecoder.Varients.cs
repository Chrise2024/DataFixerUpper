using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Codecs;

public partial interface IDecoder<T>
{
    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ITerminal"/> from this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>The default implementation returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ITerminal"/>.</returns>
    public ITerminal AsTerminal()
    {
        return new TerminalImpl<T>(this);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ITerminal"/>.
    /// </summary>
    /// <param name="terminal">The terminal decoder.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given terminal decoder.</returns>
    public static IDecoder<T> FromTerminal(ITerminal terminal)
    {
        return terminal.AsDecoder();
    }

    /// <summary>
    /// A simple decoder interface that discards any serialized input that was not used to decode the object.
    /// </summary>
    /// <remarks>In practice, there is not expected to be any remained serialized input when using objects of this type.</remarks>
    public interface ITerminal
    {
        /// <summary>
        /// Completely decodes the given serialized input and returns the decoded object in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
        /// </summary>
        /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
        /// <param name="input">The value to decode.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object.</returns>
        public DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
            where TObject : notnull;

        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that performs the same actions as this terminal decoder.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that returns the result of <see cref="M:DataFixerUpper.Serialization.Codecs.IDecoder`1.ITerminal.Decode``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},``0)"/> with an empty remainder.</returns>
        public IDecoder<T> AsDecoder()
        {
            return new TerminalDecoderImpl<T>(this);
        }
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.IBoxed"/> from this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>The default implementation returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.IBoxed"/>.</returns>
    public IBoxed AsBoxed()
    {
        return new BoxedImpl<T>(this);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.IBoxed"/>.
    /// </summary>
    /// <param name="boxed">The boxed decoder.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given terminal decoder.</returns>
    public static IDecoder<T> FromBoxed(IBoxed boxed)
    {
        return boxed.AsDecoder();
    }

    /// <summary>
    /// A simple decoder interface that decodes an object from a <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>.
    /// </summary>
    public interface IBoxed
    {
        /// <summary>
        /// Decodes the input into an object and returns the decoded object and remaining serialized data in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
        /// </summary>
        /// <param name="dynamic">The serialized data.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object and the remaining serialized data.</returns>
        public DataResult<(T, TObject?)> Decode<TObject>(Dynamic<TObject> dynamic)
            where TObject : notnull;

        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that performs the same actions as this boxed decoder.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that returns the result of <see cref="M:DataFixerUpper.Serialization.Codecs.IDecoder`1.IBoxed.Decode``1(DataFixerUpper.Serialization.DynamicOps.Dynamic{``0})"/>.</returns>
        public IDecoder<T> AsDecoder()
        {
            return new BoxedDecoderImpl<T>(this);
        }
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ISimple"/> from this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>The default implementation returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ISimple"/>.</returns>
    public ISimple AsSimple()
    {
        return new SimpleImpl<T>(this);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1.ISimple"/>.
    /// </summary>
    /// <param name="simple">The simple decoder.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> from the given terminal decoder.</returns>
    public static IDecoder<T> FromSimple(ISimple simple)
    {
        return simple.AsDecoder();
    }

    /// <summary>
    /// A simple decoder interface that completely decodes an object from a <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>, discarding any remaining serialized data.
    /// </summary>
    public interface ISimple
    {
        /// <summary>
        /// Completely decodes an object from the given <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> data. Any remaining serialized data is discarded.
        /// </summary>
        /// <param name="dynamic">The serialized data.</param>
        /// <typeparam name="TObject">The type of the encoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded object.</returns>
        public DataResult<T> Decode<TObject>(Dynamic<TObject> dynamic)
            where TObject : notnull;

        /// <summary>
        /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that performs the same actions as this simple decoder.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that returns the result of <see cref="M:DataFixerUpper.Serialization.Codecs.IDecoder`1.ISimple.Decode``1(DataFixerUpper.Serialization.DynamicOps.Dynamic{``0})"/>.</returns>
        public IDecoder<T> AsDecoder()
        {
            return new SimpleDecoderImpl<T>(this);
        }
    }
}

file sealed record TerminalImpl<T>(IDecoder<T> Decoder) : IDecoder<T>.ITerminal
{
    public DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return Decoder.Parse(ops, input);
    }
}

file sealed record TerminalDecoderImpl<T>(IDecoder<T>.ITerminal Terminal) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return Terminal.Decode(ops, input).Map(value => (value, ops.Empty()));
    }

    public override string ToString()
    {
        return $"TerminalDecoder[{Terminal}]";
    }
}

file sealed record BoxedImpl<T>(IDecoder<T> Decoder) : IDecoder<T>.IBoxed
{
    public DataResult<(T, TObject?)> Decode<TObject>(Dynamic<TObject> dynamic)
        where TObject : notnull
    {
        return Decoder.Decode(dynamic);
    }
}

file sealed record BoxedDecoderImpl<T>(IDecoder<T>.IBoxed Boxed) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return Boxed.Decode(new Dynamic<TObject>(ops, input));
    }

    public override string ToString()
    {
        return $"BoxedDecoder[{Boxed}]";
    }
}

file sealed record SimpleImpl<T>(IDecoder<T> Decoder) : IDecoder<T>.ISimple
{
    public DataResult<T> Decode<TObject>(Dynamic<TObject> dynamic)
        where TObject : notnull
    {
        return Decoder.Parse(dynamic);
    }
}

file sealed record SimpleDecoderImpl<T>(IDecoder<T>.ISimple Simple) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return Simple.Decode(new Dynamic<TObject>(ops, input)).Map(value => (value, input));
    }

    public override string ToString()
    {
        return $"SimpleDecoder[{Simple}]";
    }
}
