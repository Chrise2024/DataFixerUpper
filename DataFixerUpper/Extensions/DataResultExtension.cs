using System;
using DataFixerUpper.Datafixers.Kinds;
using DataFixerUpper.Extensions;
using DataFixerUpper.Utils;

// ReSharper disable once CheckNamespace
namespace DataFixerUpper.Serialization;

/// <summary>
/// Extension for <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.
/// </summary>
/// <seealso cref="T:DataFixerUpper.Serialization.DataResult`1"/>
public static class DataResultExtension
{
    private static readonly DataResultOperator Operator = DataResultOperator.Instance;

    private static DataResult<TResult> Unbox<TResult>(IApp<DataResult.Mu, TResult> box)
    {
        return DataResultOperator.Unbox(box);
    }

    extension<T>(DataResult<T> dataResult)
    {
        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or empty value if <see langword="false"/>.
        /// </summary>
        /// <returns>The successful result or a default value.</returns>
        public Optional<T> GetResult()
        {
            if (dataResult.TryGetResult(out T? result))
            {
                return Optional.Create(result);
            }

            return Optional<T>.Empty;
        }
        
        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or a default value if <see langword="false"/>.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the operation was not successful.</param>
        /// <returns>The successful result or a default value.</returns>
        public T GetResultOrDefault(T defaultValue)
        {
            if (dataResult.TryGetResult(out T? result))
            {
                return result;
            }

            return defaultValue;
        }
        
        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or a default value if <see langword="false"/>.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the operation was not successful.</param>
        /// <returns>The successful result or a default value.</returns>
        public T GetResultOrDefault(Provider<T> defaultValue)
        {
            if (dataResult.TryGetResult(out T? result))
            {
                return result;
            }

            return defaultValue.Get();
        }
        
        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or a default value if <see langword="false"/>.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the operation was not successful.</param>
        /// <returns>The successful result or a default value.</returns>
        public T GetResultOrDefault(ValueHolder<T> defaultValue)
        {
            if (dataResult.TryGetResult(out T? result))
            {
                return result;
            }

            return defaultValue.Value;
        }

        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or throws an exception if <see langword="false"/>.
        /// </summary>
        /// <param name="errorFactory">Function to create an exception from an error message.</param>
        /// <returns>The successful result.</returns>
        /// <exception cref="Exception">Exception provided by <paramref name="errorFactory"/>.</exception>
        public T GetResultOrThrow(Func<string, Exception> errorFactory)
        {
            if (dataResult.TryGetResult(out T? result))
            {
                return result;
            }

            throw errorFactory.Apply(dataResult.ErrorResult.Message);
        }

        /// <summary>
        /// Gets the successful result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.IsSuccess"/> property is <see langword="true"/> or throws an exception if <see langword="false"/>.
        /// </summary>
        /// <returns>The successful result.</returns>
        /// <exception cref="InvalidOperationException">Default exception.</exception>
        public T GetResultOrThrow()
        {
            return dataResult.GetResultOrThrow(msg => new InvalidOperationException(msg));
        }

        /// <summary>
        /// Gets the successful result or a partial result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/> or <see langword="null"/> if <see langword="false"/>.
        /// </summary>
        /// <param name="onError">The action to perform if there is an error.</param>
        /// <returns>The successful result or a partial result.</returns>
        public Optional<T> GetResultOrPartial(Consumer<string> onError)
        {
            if (dataResult.TryGetResultOrPartial(out T? result, onError))
            {
                return Optional.Create(result);
            }

            return Optional<T>.Empty;
        }

        /// <summary>
        /// 
        /// Gets the successful result or a partial result if <see cref="P:DataFixerUpper.Serialization.DataResult`1.HasResultOrPartial"/> property is <see langword="true"/> or <see langword="null"/> if <see langword="false"/>.
        /// </summary>
        /// <returns>The successful result or a partial result.</returns>
        public Optional<T> GetResultOrPartial()
        {
            return dataResult.GetResultOrPartial(Functions.EmptyConsumer);
        }

        /// <summary>
        /// Gets the partial result if partial result is present or throws an exception if no partial present.
        /// </summary>
        /// <returns>The partial result.</returns>
        /// <exception cref="InvalidOperationException">Default exception.</exception>
        public T GetResultOrPartialOrThrow()
        {
            return dataResult.GetResultOrPartialOrThrow(msg => new InvalidOperationException(msg));
        }

        /// <summary>
        /// Gets the partial result if partial result is present or throws an exception if no partial present.
        /// </summary>
        /// <param name="errorFactory">Function to create an exception from an error message.</param>
        /// <returns>The partial result.</returns>
        /// <exception cref="Exception">Exception provided by <paramref name="errorFactory"/>.</exception>
        public T GetResultOrPartialOrThrow(Func<string, Exception> errorFactory)
        {
            if (dataResult.TryGetResultOrPartial(out T? result))
            {
                return result;
            }

            throw errorFactory.Apply(dataResult.ErrorResult.Message);
        }

        /// <summary>
        /// Set partial value of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, or do nothing if not an error.
        /// </summary>
        /// <param name="partial">Partial value to set.</param>
        /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with given partial value.</returns>
        public DataResult<T> SetPartial(T partial)
        {
            return dataResult.SetPartial(ValueHolder.Create(partial));
        }

        /// <summary>
        /// Set partial value of this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>, or do nothing if not an error.
        /// </summary>
        /// <param name="provider">Provider of partial value to set.</param>
        /// <returns>New <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with given partial value.</returns>
        public DataResult<T> SetPartial(Provider<T> provider)
        {
            return dataResult.SetPartial(ValueHolder.Create(provider));
        }

        /// <summary>
        /// Combines this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with another result using the given function.
        /// </summary>
        /// <param name="combiner">Combine function.</param>
        /// <param name="other">Another <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="T1">Type of other <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</typeparam>
        /// <typeparam name="TCombined">The type of the value returned by <paramref name="combiner"/>.</typeparam>
        /// <returns>Combined <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
        public DataResult<TCombined> Combine<T1, TCombined>(
            Func<T, T1, TCombined> combiner,
            DataResult<T1> other
        )
        {
            return Unbox(Operator.Combine(Operator.Point(combiner), dataResult, other));
        }

        /// <summary>
        /// Combines this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with another result using the given function, under the "stable" lifecycle.
        /// </summary>
        /// <param name="combiner">Combine function.</param>
        /// <param name="other">Another <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="T1">Type of other <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</typeparam>
        /// <typeparam name="TCombined">The type of the value returned by <paramref name="combiner"/>.</typeparam>
        /// <returns>Combined <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
        public DataResult<TCombined> CombineStable<T1, TCombined>(
            Func<T, T1, TCombined> combiner,
            DataResult<T1> other
        )
        {
            DataResult<Func<T, T1, TCombined>> funcResult = Unbox(Operator.Point(combiner)).SetLifecycle(Lifecycle.Stable);
            return Unbox(Operator.Combine(funcResult, dataResult, other));
        }

        /// <summary>
        /// Combines this <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> with another 2 results using the given function.
        /// </summary>
        /// <param name="combiner">Combine function.</param>
        /// <param name="other1">First of another <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <param name="other2">Second of another <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</param>
        /// <typeparam name="T1">Type of first <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</typeparam>
        /// <typeparam name="T2">Type of second <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</typeparam>
        /// <typeparam name="TCombined">The type of the value returned by <paramref name="combiner"/>.</typeparam>
        /// <returns>Combined <see cref="T:DataFixerUpper.Serialization.DataResult`1"/>.</returns>
        public DataResult<TCombined> Combine<T1, T2, TCombined>(
            Func<T, T1, T2, TCombined> combiner,
            DataResult<T1> other1,
            DataResult<T2> other2
        )
        {
            return Unbox(Operator.Combine(Operator.Point(combiner), dataResult, other1, other2));
        }
    }
}