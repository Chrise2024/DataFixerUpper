using DataFixerUpper.Utils;

namespace DataFixerUpper.Test;

[TestClass]
public class NullableTest
{
    [TestMethod]
    public void NullableReferenceType_AllowsNullValue()
    {
        string? maybeName = null;

        Assert.IsNull(maybeName);

        maybeName = "DataFixerUpper";
        Assert.AreEqual("DataFixerUpper", maybeName);
    }

    [TestMethod]
    public void NullableValueType_AllowsNullValue()
    {
        int? maybeAge = null;

        Assert.IsFalse(maybeAge.HasValue);

        maybeAge = 42;
        Assert.IsTrue(maybeAge.HasValue);
        Assert.AreEqual(42, maybeAge.Value);
    }

    [TestMethod]
    public void Optional_TracksWhetherItHasAValue()
    {
        Optional<string> empty = Optional.Create<string>(null);
        Assert.IsFalse(empty.HasValue);

        Optional<string> present = Optional.Create("hello");
        Assert.IsTrue(present.HasValue);
        Assert.AreEqual("hello", present.Value);

        Optional<int> number = Optional.Create(42);
        Assert.IsTrue(number.HasValue);
        Assert.AreEqual(42, number.Value);

        Optional<int> missing = Optional<int>.Empty;
        Assert.IsFalse(missing.HasValue);
    }
}