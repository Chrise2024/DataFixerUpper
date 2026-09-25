using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using DataFixerUpper.Datafixers.Kinds;
using DataFixerUpper.Extensions;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization;

/// <summary>
/// Static usage for <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
/// </summary>
public static class DataResult
{
    /// <summary>
    /// The witness type base of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    public abstract class Mu : Anchor;

    /// <summary>
    /// Unbox <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/> container.
    /// </summary>
    /// <param name="box">Boxed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
    /// <typeparam name="T">Wrapped type of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</typeparam>
    /// <returns>Unboxed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> Unbox<T>(IApp<Mu, T> box)
    {
        return (DataResult<T>) box;
    }

    /// <summary>
    /// Creates a successful <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>A successful <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateSuccess<T>(T result)
    {
        return CreateSuccess(result, Lifecycle.Experimental);
    }
    
    /// <summary>
    /// Creates a successful <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the given <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result value.</param>
    /// <param name="lifecycle">The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>A successful <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateSuccess<T>(T result, Lifecycle lifecycle)
    {
        return new Success<T>(result, lifecycle);
    }

    /// <summary>
    /// Creates an error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the given message.
    /// The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="messageHolder">Holder of message.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>An error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateError<T>(ValueHolder<string> messageHolder)
    {
        return CreateError(messageHolder, Optional<T>.Empty, Lifecycle.Experimental);
    }

    /// <summary>
    /// Creates an error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the given message and lifecycle.
    /// The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="messageHolder">Holder of message.</param>
    /// <param name="lifecycle">The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>An error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateError<T>(ValueHolder<string> messageHolder, Lifecycle lifecycle)
    {
        return CreateError(messageHolder, Optional<T>.Empty, lifecycle);
    }

    /// <summary>
    /// Creates an error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the given message and partial result.
    /// The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="messageHolder">Holder of message.</param>
    /// <param name="partial">The partial or fallback result value.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>An error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateError<T>(ValueHolder<string> messageHolder, Optional<T> partial)
    {
        return CreateError(messageHolder, partial, Lifecycle.Experimental);
    }

    /// <summary>
    /// Creates an error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the given message, partial result and lifecycle.
    /// </summary>
    /// <param name="messageHolder">Holder of message.</param>
    /// <param name="partial">The partial or fallback result value.</param>
    /// <param name="lifecycle">The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
    /// <typeparam name="T">The type of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> value.</typeparam>
    /// <returns>An error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static DataResult<T> CreateError<T>(ValueHolder<string> messageHolder, Optional<T> partial, Lifecycle lifecycle)
    {
        return new Error<T>(messageHolder, partial, lifecycle);
    }

    /// <summary>
    /// Converts a partial function into a function that produces a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>. 
    /// </summary>
    /// <remarks>
    /// If the partial function returns <see langword="null"/>, then the returned function returns an error <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, otherwise a successful <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </remarks>
    /// <param name="getter">The partial function.</param>
    /// <param name="errorPrefix">The error string to use if <paramref name="getter"/> returns <see langword="null"/>.</param>
    /// <typeparam name="TName">The argument type of <paramref name="getter"/>.</typeparam>
    /// <typeparam name="T">The return type of <paramref name="getter"/>.</typeparam>
    /// <returns>A function that wraps the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> of the partial function in a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public static Func<TName, DataResult<T>> PartialGet<TName, T>(Func<TName, Optional<T>> getter, ValueHolder<string> errorPrefix)
    {
        return name => getter.Apply(name)
            .Select(CreateSuccess)
            .GetOrDefault(CreateError<T>(ValueHolder.Create(() => errorPrefix.Value + name)));
    }

    /// <summary>
    /// Represents a successful result in the data.
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    public sealed class Success<T> : DataResult<T>
    {
        /// <summary>
        /// Create from <paramref name="result"/> and <paramref name="lifecycle"/>
        /// </summary>
        /// <param name="result">Result to wrap.</param>
        /// <param name="lifecycle">Lifecycle of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/></param>
        /// <exception cref="ArgumentNullException">Any argument is null.</exception>
        public Success(T result, Lifecycle lifecycle)
        {
            ArgumentNullException.ThrowIfNull(result);
            ArgumentNullException.ThrowIfNull(lifecycle);
            Result = result;
            Lifecycle = lifecycle;
        }
        
        /// <inheritdoc/>
        public override bool IsSuccess => true;
        
        /// <summary>
        /// Represents self.
        /// </summary>
        [JsonIgnore]
        public override Success<T> SuccessResult => this;

        /// <summary>
        /// Error is always <see langword="null"/> if success.
        /// </summary>
        [JsonIgnore]
        public override Error<T>? ErrorResult => null;

        /// <summary>
        /// Wrapped result.
        /// </summary>
        [NotNull]
        public T Result { get; }

        /// <inheritdoc/>
        public override Lifecycle Lifecycle { get; }
        
        /// <inheritdoc/>
        public override bool HasResultOrPartial => true;

        /// <inheritdoc/>
        public override bool TryGetResult([NotNullWhen(true)] out T? result)
        {
            result = Result;
            return true;
        }
        
        /// <inheritdoc/>
        public override bool TryGetResultOrPartial([NotNullWhen(true)] out T? result)
        {
            result = Result;
            return true;
        }
        
        /// <inheritdoc/>
        public override bool TryGetResultOrPartial([NotNullWhen(true)] out T? result, Consumer<string> onError)
        {
            result = Result;
            return true;
        }

        /// <inheritdoc/>
        public override DataResult<T> IfSuccess(Consumer<T> ifSuccess)
        {
            ifSuccess.Accept(Result);
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<T> IfError(Consumer<Error<T>> ifError)
        {
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<T> PromotePartial(Consumer<string> onError)
        {
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return new Success<TResult>(mapper.Apply(Result), Lifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<TResult> Map<TResult>(DataResult<Func<T, TResult>> mapperResult)
        {
            Lifecycle combinedLifecycle = Lifecycle + mapperResult.Lifecycle;
            if (mapperResult.TryGetResult(out Func<T, TResult>? func))
            {
                return CreateSuccess(func.Apply(Result), combinedLifecycle);
            }

            Error<Func<T, TResult>> errorResult = mapperResult.ErrorResult;
            Optional<Func<T, TResult>> partialMapper = errorResult.Partial;
            
            return CreateError(errorResult.MessageHolder, partialMapper.Select(m => m.Apply(Result)), combinedLifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<TResult> FlatMap<TResult>(Func<T, DataResult<TResult>> mapper)
        {
            return mapper.Apply(Result).AddLifecycle(Lifecycle);
        }

        /// <inheritdoc/>
        public override TResult MapOrDefault<TResult>(Func<T, TResult> mapper, Func<Error<T>, TResult> errorMapper)
        {
            return mapper.Apply(Result);
        }

        /// <inheritdoc/>
        public override DataResult<T> SetPartial(ValueHolder<T> partial)
        {
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<T> MapError(UnaryOperation<string> mapper)
        {
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<T> SetLifecycle(Lifecycle lifecycle)
        {
            return lifecycle == Lifecycle ? this : new Success<T>(Result, lifecycle);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"DataResult[{Result}]";
        }
    }

    /// <summary>
    /// Represents an error in the data.
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    public sealed class Error<T> : DataResult<T>
    {
        /// <summary>
        /// Create from message, partial value and lifecycle.
        /// </summary>
        /// <param name="messageHolder">Holder of message.</param>
        /// <param name="partial">Partial value.</param>
        /// <param name="lifecycle">Lifecycle of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        public Error(ValueHolder<string> messageHolder, Optional<T> partial, Lifecycle lifecycle)
        {
            ArgumentNullException.ThrowIfNull(messageHolder);
            ArgumentNullException.ThrowIfNull(lifecycle);
            MessageHolder = messageHolder;
            Lifecycle = lifecycle;
            Partial = partial;
        }

        internal readonly ValueHolder<string> MessageHolder;

        /// <inheritdoc/>
        public override bool IsSuccess => false;

        /// <inheritdoc/>
        [JsonIgnore]
        public override Success<T>? SuccessResult => null;
        
        /// <summary>
        /// Represents self.
        /// </summary>
        [JsonIgnore]
        public override Error<T> ErrorResult => this;

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message => MessageHolder.Value;

        /// <summary>
        /// Partial value of result.
        /// </summary>
        public Optional<T> Partial { get; }
        
        /// <inheritdoc/>
        public override Lifecycle Lifecycle { get; }

        /// <inheritdoc/>
        public override bool HasResultOrPartial => Partial.HasValue;

        /// <inheritdoc/>
        public override DataResult<T> IfSuccess(Consumer<T> ifSuccess)
        {
            return this;
        }

        /// <inheritdoc/>
        public override bool TryGetResult([NotNullWhen(true)] out T? result)
        {
            result = default;
            return false;
        }

        /// <inheritdoc/>
        public override bool TryGetResultOrPartial([NotNullWhen(true)] out T? result)
        {
            result = Partial.GetOrDefault();
            return Partial.HasValue;
        }

        /// <inheritdoc/>
        public override bool TryGetResultOrPartial([NotNullWhen(true)] out T? result, Consumer<string> onError)
        {
            onError.Accept(Message);
            result = Partial.GetOrDefault();
            return Partial.HasValue;
        }

        /// <inheritdoc/>
        public override DataResult<T> IfError(Consumer<Error<T>> ifError)
        {
            ifError.Accept(this);
            return this;
        }

        /// <inheritdoc/>
        public override DataResult<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            return new Error<TResult>(MessageHolder, Partial.Select(mapper), Lifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<TResult> Map<TResult>(
            DataResult<Func<T, TResult>> mapperResult
        )
        {
            Lifecycle combinedLifecycle = Lifecycle + mapperResult.Lifecycle;
            if (mapperResult.TryGetResult(out Func<T, TResult>? func))
            {
                return CreateError(MessageHolder, Partial.Select(func), combinedLifecycle);
            }

            Error<Func<T, TResult>> errorResult = mapperResult.ErrorResult;
            Optional<Func<T, TResult>> partialMapper = errorResult.Partial;
            Optional<TResult> mr = partialMapper.HasValue && Partial.HasValue ? Partial.Select(partialMapper.Value) : Optional<TResult>.Empty;
            return CreateError(Message + ';' + errorResult.Message, mr, combinedLifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<TResult> FlatMap<TResult>(Func<T, DataResult<TResult>> mapper)
        {
            if (!Partial.HasValue)
            {
                return new Error<TResult>(MessageHolder, Optional<TResult>.Empty, Lifecycle);
            }

            DataResult<TResult> other = mapper.Apply(Partial.Value);
            Lifecycle combinedLifecycle = Lifecycle + other.Lifecycle;

            if (other.TryGetResult(out TResult? result))
            {
                return new Error<TResult>(MessageHolder, Optional.Create(result), Lifecycle);
            }

            Error<TResult> otherError = other.ErrorResult;
            return new Error<TResult>(Message + ';' + otherError.Message, otherError.Partial, combinedLifecycle);
        }

        /// <inheritdoc/>
        public override TResult MapOrDefault<TResult>(Func<T, TResult> mapper, Func<Error<T>, TResult> errorMapper)
        {
            return errorMapper.Apply(this);
        }

        /// <inheritdoc/>
        public override DataResult<T> PromotePartial(Consumer<string> onError)
        {
            onError.Accept(Message);
            return Partial.HasValue ? new Success<T>(Partial.Value, Lifecycle) : this;
        }

        /// <inheritdoc/>
        public override DataResult<T> SetPartial(ValueHolder<T> partial)
        {
            return new Error<T>(MessageHolder, Optional.Create(partial.Value), Lifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<T> MapError(UnaryOperation<string> mapper)
        {
            return new Error<T>(ValueHolder.Create(() => mapper.Apply(Message)), Partial, Lifecycle);
        }

        /// <inheritdoc/>
        public override DataResult<T> SetLifecycle(Lifecycle lifecycle)
        {
            return lifecycle == Lifecycle ? this : new Error<T>(MessageHolder, Partial, lifecycle);
        }
    }
}

/// <summary>
/// Represents either a <seealso cref="T:DataFixerUpper.Serialization.DataResult.Success`1">successful operation</seealso>, or a  <seealso cref="T:DataFixerUpper.Serialization.DataResult.Error`1">partial operation</seealso> with an error message and a partial result (if available).
/// <remarks>Also stores an additional lifecycle marker (monoidal).</remarks>
/// </summary>
/// <typeparam name="T">The type of the wrapped result.</typeparam>
public abstract class DataResult<T> : IApp<DataResult.Mu, T>
{
    private protected DataResult() { }
    
    /// <summary>
    /// Gets the lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    public abstract Lifecycle Lifecycle { get; }
    
    /// <summary>
    /// Gets the success result if the operation was successful.
    /// </summary>
    [JsonIgnore]
    public abstract DataResult.Success<T>? SuccessResult { get; }
    
    /// <summary>
    /// Gets the error result if the operation was not successful.
    /// </summary>
    [JsonIgnore]
    public abstract DataResult.Error<T>? ErrorResult { get; }
    
    /// <summary>
    /// Gets a value indicating whether the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> has a successful result or a partial result.
    /// </summary>
    public abstract bool HasResultOrPartial { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is successful.
    /// </summary>
    [MemberNotNullWhen(true, nameof(SuccessResult))]
    [MemberNotNullWhen(false, nameof(ErrorResult))]
    public abstract bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is an error.
    /// </summary>
    [MemberNotNullWhen(false, nameof(SuccessResult))]
    [MemberNotNullWhen(true, nameof(ErrorResult))]
    public bool IsError => !IsSuccess;

    /// <summary>
    /// Gets the result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="result">When this method returns, the result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/>; otherwise, the default value. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/>; otherwise, <see langword="false" />.</returns>
    [MemberNotNullWhen(false, nameof(ErrorResult))]
    public abstract bool TryGetResult([NotNullWhen(true)] out T? result);


    /// <summary>
    /// Gets the result or partial result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="result">When this method returns, the result or partial result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/>; otherwise, the default value. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/>; otherwise, <see langword="false" />.</returns>
    [MemberNotNullWhen(false, nameof(ErrorResult))]
    public abstract bool TryGetResultOrPartial([NotNullWhen(true)] out T? result);

    /// <summary>
    /// Gets the result or partial result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="result">When this method returns, the result or partial result of <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/>; otherwise, the default value. This parameter is passed uninitialized.</param>
    /// <param name="onError">The action to perform if there is an error.</param>
    /// <returns><see langword="true"/> if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/>; otherwise, <see langword="false" />.</returns>
    [MemberNotNullWhen(false, nameof(ErrorResult))]
    public abstract bool TryGetResultOrPartial([NotNullWhen(true)] out T? result, Consumer<string> onError);
    
    /// <summary>
    /// If <see cref="M:IsSuccess"/> returns <see langword="true"/> perform the given action on the success result.
    /// </summary>
    /// <param name="ifSuccess">Action to perform.</param>
    /// <returns>Self.</returns>
    public abstract DataResult<T> IfSuccess(Consumer<T> ifSuccess);
    
    /// <summary>
    /// If <see cref="M:IsError"/> returns <see langword="true"/> perform the given action on this <see cref="T:DataFixerUpper.Serialization.DataResult.Error`1"/>.
    /// </summary>
    /// <param name="ifError">Action to perform.</param>
    /// <returns>Self.</returns>
    public abstract DataResult<T> IfError(Consumer<DataResult.Error<T>> ifError);
    
    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="mapper">Transformation function.</param>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="mapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public abstract DataResult<TResult> Map<TResult>(Func<T, TResult> mapper);
    
    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="mapperResult">Wrapped transformation function.</param>
    /// <typeparam name="TResult">The type of the value returned by inner function of <paramref name="mapperResult"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public abstract DataResult<TResult> Map<TResult>(DataResult<Func<T, TResult>> mapperResult);
    
    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> and flat the nested <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
    /// </summary>
    /// <param name="mapper">Transformation function.</param>
    /// <typeparam name="TResult">The type of the inner value returned by <paramref name="mapper"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public abstract DataResult<TResult> FlatMap<TResult>(Func<T, DataResult<TResult>> mapper);
    
    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, if not a success returns value via the <see cref="T:DataFixerUpper.Serialization.DataResult.Error`1"/>.
    /// </summary>
    /// <param name="mapper">Transformation function.</param>
    /// <param name="errorMapper">Transformation function of <see cref="T:DataFixerUpper.Serialization.DataResult.Error`1"/>.</param>
    /// <typeparam name="TResult">The type of the inner value returned by <paramref name="mapper"/>.</typeparam>
    /// <returns>Transformed value of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> or transformed from <see cref="T:DataFixerUpper.Serialization.DataResult.Error`1"/>.</returns>
    public abstract TResult MapOrDefault<TResult>(Func<T, TResult> mapper, Func<DataResult.Error<T>, TResult> errorMapper);
    
    /// <summary>
    /// Promotes an error with a partial result to a success. If this is a success, it is returned unchanged.
    /// </summary>
    /// <param name="onError">A callback to run on error. It is passed the error string.</param>
    /// <returns>New success <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public abstract DataResult<T> PromotePartial(Consumer<string> onError);
    
    /// <summary>
    /// Set partial value of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, or do nothing if not an error.
    /// </summary>
    /// <param name="partial">Partial value to set.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with given partial value.</returns>
    public abstract DataResult<T> SetPartial(ValueHolder<T> partial);
    
    /// <summary>
    /// Applies the given function to the error message contained in this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, or do nothing if not an error.
    /// </summary>
    /// <param name="mapper">Operation on error message.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with transformed error message.</returns>
    public abstract DataResult<T> MapError(UnaryOperation<string> mapper);
    
    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the same value as this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, but with the provided lifecycle.
    /// </summary>
    /// <param name="lifecycle">The <see cref="T:DataFixerUpper.Serialization.Lifecycle"/> to set.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with given <see cref="T:DataFixerUpper.Serialization.Lifecycle"/>.</returns>
    public abstract DataResult<T> SetLifecycle(Lifecycle lifecycle);

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with the same value as this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, but with the provided lifecycle added to this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>'s lifecycle.
    /// </summary>
    /// <param name="lifecycle">The <see cref="T:DataFixerUpper.Serialization.Lifecycle"/> to add.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with added <see cref="T:DataFixerUpper.Serialization.Lifecycle"/>.</returns>
    public DataResult<T> AddLifecycle(Lifecycle lifecycle)
    {
        return SetLifecycle(Lifecycle + lifecycle);
    }
}

/// <summary>
/// Operator for <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
/// </summary>
public sealed class DataResultOperator : Applicative<DataResult.Mu, DataResultOperator.Mu>
{
    /// <summary>
    /// Instance of <see cref="T:DataFixerUpper.Serialization.DataResultOperator"/>.
    /// </summary>
    public static DataResultOperator Instance => new();

    /// <inheritdoc/>
    public abstract class Mu : Applicative.Mu;
    
    /// <summary>
    /// Unbox boxed <see cref="T:DataFixerUpper.Datafixers.Kinds.IApp`2"/>.
    /// </summary>
    /// <param name="box">Boxed value.</param>
    /// <typeparam name="T">The type of the wrapped result.</typeparam>
    /// <returns>Unboxed <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
    public new static DataResult<T> Unbox<T>(IApp<DataResult.Mu, T> box)
    {
        return DataResult.Unbox(box);
    }

    /// <summary>
    /// Creates a successful result from given value.
    /// The lifecycle of the <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> is <see cref="P:DataFixerUpper.Serialization.Lifecycle.Experimental"/>.
    /// </summary>
    /// <param name="result">Value to wrap.</param>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>Wrapped result.</returns>
    public override IApp<DataResult.Mu, T> Point<T>(T result)
    {
        return DataResult.CreateSuccess(result);
    }

    /// <inheritdoc/>
    public override Func<IApp<DataResult.Mu, T1>, IApp<DataResult.Mu, T2>> Lift<T1, T2>(IApp<DataResult.Mu, Func<T1, T2>> function)
    {
        return t1 => Select(function, t1);
    }

    /// <inheritdoc/>
    public override IApp<DataResult.Mu, T2> Select<T1, T2>(Func<T1, T2> selector, IApp<DataResult.Mu, T1> target)
    {
        return Unbox(target).Map(selector);
    }

    /// <inheritdoc/>
    public override IApp<DataResult.Mu, T2> Select<T1, T2>(IApp<DataResult.Mu, Func<T1, T2>> selector, IApp<DataResult.Mu, T1> app)
    {
        return Unbox(app).Map(Unbox(selector));
    }

    /// <inheritdoc/>
    public override IApp<DataResult.Mu, TR> Combine<T1, T2, TR>(IApp<DataResult.Mu, Func<T1, T2, TR>> combiner, IApp<DataResult.Mu, T1> t1, IApp<DataResult.Mu, T2> t2)
    {
        DataResult<Func<T1, T2, TR>> combinerResult = Unbox(combiner);
        DataResult<T1> r1 = Unbox(t1);
        DataResult<T2> r2 = Unbox(t2);

        if (combinerResult.IsSuccess && r1.IsSuccess && r2.IsSuccess)
        {
            return DataResult.CreateSuccess(
                combinerResult.SuccessResult.Result.Apply(r1.SuccessResult.Result, r2.SuccessResult.Result),
                combinerResult.Lifecycle + r1.Lifecycle + r2.Lifecycle
            );
        }

        return base.Combine(combiner, t1, t2);
    }

    /// <inheritdoc/>
    public override IApp<DataResult.Mu, TR> Combine<T1, T2, T3, TR>(IApp<DataResult.Mu, Func<T1, T2, T3, TR>> combiner, IApp<DataResult.Mu, T1> t1, IApp<DataResult.Mu, T2> t2, IApp<DataResult.Mu, T3> t3)
    {
        DataResult<Func<T1, T2, T3, TR>> combinerResult = Unbox(combiner);
        DataResult<T1> r1 = Unbox(t1);
        DataResult<T2> r2 = Unbox(t2);
        DataResult<T3> r3 = Unbox(t3);

        if (combinerResult.IsSuccess && r1.IsSuccess && r2.IsSuccess && r3.IsSuccess)
        {
            return DataResult.CreateSuccess(
                combinerResult.SuccessResult.Result.Apply(r1.SuccessResult.Result, r2.SuccessResult.Result, r3.SuccessResult.Result),
                combinerResult.Lifecycle + r1.Lifecycle + r2.Lifecycle + r3.Lifecycle
            );
        }

        return base.Combine(combiner, t1, t2, t3);
    }
}