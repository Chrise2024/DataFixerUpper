using System;
using System.Diagnostics.CodeAnalysis;
using DataFixerUpper.Extensions;

namespace DataFixerUpper.Utils;

/// <summary>
/// Operations for <see cref="T:DataFixerUpper.Utils.Either`2"/>.
/// </summary>
public static class Either
{
    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.Either`2"/> instance containing a left value.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <typeparam name="TL">Left type.</typeparam>
    /// <typeparam name="TR">Right type.</typeparam>
    /// <returns>New <see cref="T:DataFixerUpper.Utils.Either`2"/> instance contains left value.</returns>
    public static Either<TL, TR> CreateLeft<TL, TR>(TL left)
    {
        return new EitherLeft<TL, TR>(left);
    }

    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.Either`2"/> instance containing a right value.
    /// </summary>
    /// <param name="right">The right value.</param>
    /// <typeparam name="TL">Left type.</typeparam>
    /// <typeparam name="TR">Right type.</typeparam>
    /// <returns>New <see cref="T:DataFixerUpper.Utils.Either`2"/> instance contains right value.</returns>
    public static Either<TL, TR> CreateRight<TL, TR>(TR right)
    {
        return new EitherRight<TL, TR>(right);
    }

    /// <summary>
    /// Unwraps the value from an <see cref="T:DataFixerUpper.Utils.Either`2"/> instance which has the same type for both left and right values.
    /// </summary>
    /// <param name="either"><see cref="T:DataFixerUpper.Utils.Either`2"/> instance.</param>
    /// <typeparam name="T">Common type.</typeparam>
    /// <returns>Unwrapped value.</returns>
    public static T Unwrap<T>(Either<T, T> either)
    {
        return either.MapGet(Functions.Identity, Functions.Identity);
    }
}

/// <summary>
/// Represents a type which may hold either a value of the left type, or a value of the right type. 
/// </summary>
/// <typeparam name="TL">Left type</typeparam>
/// <typeparam name="TR">Right type</typeparam>
public abstract record Either<TL, TR>
{
    private protected Either() { }

    /// <summary>
    /// Get the left value.
    /// </summary>
    /// <value>Left value.</value>
    public abstract TL? Left { get; }


    /// <summary>
    /// Gets a value indicating whether the current <see cref="T:DataFixerUpper.Utils.Either`2"/> has left value or not.
    /// </summary>
    /// <returns>
    /// <see langword="true"/>If the current <see cref="T:DataFixerUpper.Utils.Either`2"/> object has left value; <see langword="false"/> if the current <see cref="T:DataFixerUpper.Utils.Either`2"/> object has right value.
    /// </returns>
    [MemberNotNullWhen(true, nameof(Left))]
    public abstract bool HasLeft { get; }

    /// <summary>
    /// Get the right value.
    /// </summary>
    /// <value>Right value.</value>
    public abstract TR? Right { get; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="T:DataFixerUpper.Utils.Either`2"/> has right value or not.
    /// </summary>
    /// <returns>
    /// <see langword="true"/>If the current <see cref="T:DataFixerUpper.Utils.Either`2"/> object has right value, <see langword="false"/> if the current <see cref="T:DataFixerUpper.Utils.Either`2"/> object has left value.
    /// </returns>
    [MemberNotNullWhen(true, nameof(Right))]
    public abstract bool HasRight { get; }

    /// <summary>
    ///  Transform either the left or the right value, whichever is present, to another type.
    /// </summary>
    /// <param name="lMapper">Transformation function of left.</param>
    /// <param name="rMapper">Transformation function of right.</param>
    /// <typeparam name="TL1">The type of the value returned by <paramref name="lMapper"/>.</typeparam>
    /// <typeparam name="TR1">The type of the value returned by <paramref name="rMapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Either`2"/>.</returns>
    public abstract Either<TL1, TR1> MapBoth<TL1, TR1>(Func<TL, TL1> lMapper, Func<TR, TR1> rMapper);

    /// <summary>
    /// Transform the left value if <see cref="P:DataFixerUpper.Utils.Either`2.HasLeft"/> property is <see langword="true"/>, or do nothing.
    /// </summary>
    /// <param name="lMapper">Transformation function of left.</param>
    /// <typeparam name="TL1">The type of the value returned by <paramref name="lMapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Either`2"/>.</returns>
    public Either<TL1, TR> MapLeft<TL1>(Func<TL, TL1> lMapper)
    {
        return MapBoth(lMapper, Functions.Identity);
    }

    /// <summary>
    /// Transform the right value if <see cref="P:DataFixerUpper.Utils.Either`2.HasRight"/> property is <see langword="true"/>, or do nothing.
    /// </summary>
    /// <param name="rMapper">Transformation function of right.</param>
    /// <typeparam name="TR1">The type of the value returned by <paramref name="rMapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Either`2"/>.</returns>
    public Either<TL, TR1> MapRight<TR1>(Func<TR, TR1> rMapper)
    {
        return MapBoth(Functions.Identity, rMapper);
    }

    /// <summary>
    /// Transform either the left or the right value into common type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="lMapper">Transformation function of left.</param>
    /// <param name="rMapper">Transformation function of right.</param>
    /// <typeparam name="T">Common type returned by both <paramref name="lMapper"/> and <paramref name="rMapper"/>.</typeparam>
    /// <returns>Transformed value.</returns>
    public abstract T MapGet<T>(Func<TL, T> lMapper, Func<TR, T> rMapper);

    /// <summary>
    /// Applies the given callback if <see cref="P:DataFixerUpper.Utils.Either`2.HasLeft"/> property is <see langword="true"/>.
    /// </summary>
    /// <param name="ifAction">Callback to apply to the left value.</param>
    /// <returns>Self.</returns>
    public abstract Either<TL, TR> IfLeft(Consumer<TL> ifAction);

    /// <summary>
    /// Applies the given callback if <see cref="P:DataFixerUpper.Utils.Either`2.HasRight"/> property is <see langword="true"/>.
    /// </summary>
    /// <param name="ifAction">Callback to apply to the right value.</param>
    /// <returns>Self.</returns>
    public abstract Either<TL, TR> IfRight(Consumer<TR> ifAction);

    /// <summary>
    /// Returns the left value if <see cref="P:DataFixerUpper.Utils.Either`2.HasLeft"/> property is <see langword="true"/>, or throws an exception if it is not. If the <see cref="P:Right">right value</see> is an instance of <see cref="Exception"/>, the thrown exception has that value as its cause.
    /// </summary>
    /// <returns>The left value.</returns>
    /// <exception cref="SystemException"> If <see cref="P:DataFixerUpper.Utils.Either`2.HasLeft"/> property is <see langword="false"/>.</exception>
    public TL OrThrow()
    {
        return MapGet(
            Functions.Identity, r =>
            {
                if (r is Exception e)
                {
                    throw new SystemException(e.Message, e);
                }

                throw new SystemException(r?.ToString());
            }
        );
    }

    /// <summary>
    /// Transform the left value, if <see cref="P:DataFixerUpper.Utils.Either`2.HasLeft"/> property is <see langword="true"/>, to a new type in an <see cref="T:DataFixerUpper.Utils.Either`2"/>.
    /// </summary>
    /// <param name="mapper">Transformation function.</param>
    /// <typeparam name="TL1">The type of the value returned by <paramref name="mapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Either`2"/>.</returns>
    public Either<TL1, TR> FlatMap<TL1>(Func<TL, Either<TL1, TR>> mapper)
    {
        return MapGet(mapper, Either.CreateRight<TL1, TR>);
    }

    /// <summary>
    /// Swaps the value in <see cref="T:DataFixerUpper.Utils.Either`2"/>, such that the left value becomes the right value and visa-versa.
    /// </summary>
    /// <returns>Swapped <see cref="T:DataFixerUpper.Utils.Either`2"/>.</returns>
    public Either<TR, TL> Swap()
    {
        return MapGet(Either.CreateRight<TR, TL>, Either.CreateLeft<TR, TL>);
    }
}

file sealed record EitherLeft<TL, TR> : Either<TL, TR>
{
    private readonly TL _left;

    public override TL Left => _left;

    public override bool HasLeft => true;

    public override TR? Right => default;

    public override bool HasRight => false;

    public EitherLeft(TL left)
    {
        ArgumentNullException.ThrowIfNull(left);
        _left = left;
    }

    public override Either<TL1, TR1> MapBoth<TL1, TR1>(Func<TL, TL1> lMapper, Func<TR, TR1> rMapper)
    {
        return new EitherLeft<TL1, TR1>(lMapper.Apply(_left));
    }

    public override T MapGet<T>(Func<TL, T> lMapper, Func<TR, T> rMapper)
    {
        return lMapper.Apply(_left);
    }

    public override Either<TL, TR> IfLeft(Consumer<TL> ifAction)
    {
        ifAction.Accept(_left);
        return this;
    }

    public override Either<TL, TR> IfRight(Consumer<TR> ifAction)
    {
        return this;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(EitherSide.Left);
        hash.Add(_left);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"Left[{_left}]";
    }
}

file sealed record EitherRight<TL, TR> : Either<TL, TR>
{
    private readonly TR _right;

    public override TL? Left => default;

    public override bool HasLeft => false;

    public override TR Right => _right;

    public override bool HasRight => true;

    public EitherRight(TR right)
    {
        ArgumentNullException.ThrowIfNull(right);
        _right = right;
    }

    public override Either<TL1, TR1> MapBoth<TL1, TR1>(Func<TL, TL1> lMapper, Func<TR, TR1> rMapper)
    {
        return new EitherRight<TL1, TR1>(rMapper.Apply(_right));
    }

    public override T MapGet<T>(Func<TL, T> lMapper, Func<TR, T> rMapper)
    {
        return rMapper.Apply(_right);
    }

    public override Either<TL, TR> IfLeft(Consumer<TL> ifAction)
    {
        return this;
    }

    public override Either<TL, TR> IfRight(Consumer<TR> ifAction)
    {
        ifAction.Accept(_right);
        return this;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(EitherSide.Right);
        hash.Add(_right);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"Right[{_right}]";
    }
}

file enum EitherSide
{
    Left,
    Right
}