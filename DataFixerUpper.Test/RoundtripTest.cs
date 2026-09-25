using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.Codecs.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Test;

// Incompleted
// Exists precision loss
[TestClass]
public class RoundtripTest
{
    private record TestData(
        float a,
        double b,
        byte c,
        short d,
        int e,
        long f,
        bool g,
        string h,
        IList<string> i,
        IDictionary<string, string> j
    ) {
        public static Codec<TestData> CODEC = RecordCodecBuilder.CreateCodec<TestData>(i => i.Group(
            Codec.Float.Field("a").ForGetter<TestData>(d => d.a),
            Codec.Double.Field("b").ForGetter<TestData>(d => d.b),
            Codec.Byte.Field("c").ForGetter<TestData>(d => d.c),
            Codec.Short.Field("d").ForGetter<TestData>(d => d.d),
            Codec.Int.Field("e").ForGetter<TestData>(d => d.e),
            Codec.Long.Field("f").ForGetter<TestData>(d => d.f),
            Codec.Bool.Field("g").ForGetter<TestData>(d => d.g),
            Codec.String.Field("h").ForGetter<TestData>(d => d.h),
            Codec.String.List().Field("i").ForGetter<TestData>(d => d.i),
            Codec.CreateUnboundedDictionary(Codec.String, Codec.String).Field("j").ForGetter<TestData>(d => d.j)
        ).Apply(i, (a, b, c, d, e, f, g, h, i, j) => new TestData(a, b, c, d, e, f, g, h, i, j)));
    }

    private static TestData MakeRandomTestData() {
        Random random = new (DateTime.Now.Microsecond);
        return new TestData(
            random.NextSingle(),
            random.NextDouble(),
            (byte) random.Next(),
            (short) random.Next(),
            random.Next(),
            random.NextInt64(),
            random.Next() % 2 == 0,
            random.NextSingle().ToString(CultureInfo.InvariantCulture),
            Enumerable.Range(0, random.Next(100))
                .Select(_ => random.NextSingle().ToString(CultureInfo.InvariantCulture))
                .ToImmutableList(),
            Enumerable.Range(0, random.Next(100))
                .ToImmutableDictionary(
                    _ => random.NextSingle().ToString(CultureInfo.InvariantCulture),
                    _ => random.NextSingle().ToString(CultureInfo.InvariantCulture)
                )
        );
    }

    private void TestWriteRead<T>(DynamicOps<T> ops)
        where T : notnull
    {
        TestData data = MakeRandomTestData();

        DataResult<T> encoded = TestData.CODEC.EncodeStart(ops, data);
        DataResult<TestData> decoded = encoded.FlatMap(r => TestData.CODEC.Parse(ops, r));

        Assert.AreEqual(JsonSerializer.Serialize(data), JsonSerializer.Serialize(decoded.GetResultOrThrow()), "read(write(x)) == x");
    }

    private void TestReadWrite<T>(DynamicOps<T> ops)
        where T : notnull
    {
        TestData data = MakeRandomTestData();

        DataResult<T> encoded = TestData.CODEC.EncodeStart(ops, data);
        DataResult<TestData> decoded = encoded.FlatMap(r => TestData.CODEC.Parse(ops, r));
        DataResult<T> reEncoded = decoded.FlatMap(r => TestData.CODEC.EncodeStart(ops, r));
        
        Assert.AreEqual(JsonSerializer.Serialize(data), JsonSerializer.Serialize(reEncoded), "write(read(x)) == x");
    }

    [TestMethod]
    public void TestWriteReadJson()
    {
        TestWriteRead(JsonOps.Instance);
    }

    [TestMethod]
    public void TestReadWriteJson()
    {
        TestReadWrite(JsonOps.Instance);
    }

    [TestMethod]
    public void TestWriteReadDotnet() {
        TestWriteRead(DotnetOps.Instance);
    }

    [TestMethod]
    public void TestReadWriteDotnet() {
        TestReadWrite(DotnetOps.Instance);
    }
}