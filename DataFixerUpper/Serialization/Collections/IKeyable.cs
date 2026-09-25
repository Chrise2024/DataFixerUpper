using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// A definer or acceptor of serialized keys.
/// </summary>
/// <remarks>
/// Types implementing this interface define a set of valid keys. Typically, these keys are serialized to or extracted from some serialized object.
/// </remarks>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>
public interface IKeyable
{
    /// <summary>
    /// Returns the set of keys this object defines or accepts, serialized to the provided form.
    /// </summary>
    /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to serialize values.</param>
    /// <typeparam name="TObject">The type this class serializes to and deserializes from.</typeparam>
    /// <returns>The set of keys this object defines.</returns>
    IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull;

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Collections.IKeyable"/> that defines the keys supplied by the argument.
    /// </summary>
    /// <param name="keysProvider">A supplier of keys. A fresh <see cref="T:System.Collections.Generic.IEnumerable`1"/> should be returned on each invocation.</param>
    /// <typeparam name="TObject">The type this class serializes to and deserializes from.</typeparam>
    /// <returns></returns>
    public static IKeyable ForStrings<TObject>(Provider<IEnumerable<string>> keysProvider)
    {
        return new ForStringsImpl(keysProvider);
    }

    private sealed record ForStringsImpl(Provider<IEnumerable<string>> KeysFactory) : IKeyable
    {
        public IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
            where TObject : notnull
        {
            return KeysFactory.Get().Select(ops.CreateString);
        }
    }
}