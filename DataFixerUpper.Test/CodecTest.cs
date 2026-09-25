using System.Collections;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.Codecs.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Test;

#nullable disable

[TestClass]
public class CodecTest
{
    #region Utils
    
    private static readonly Codec<string> ToLowerCase = Codec.String.XMap(s => s.ToLowerInvariant(), s => s.ToLowerInvariant());

    private static readonly IEqualityComparer<object> TestComparer = new TestComparer();
    
    private static object ToDotnet<T>(Codec<T> codec, T value) {
        return codec.EncodeStart(DotnetOps.Instance, value).GetResultOrThrow(m => new AssertFailedException(m));
    }

    private static T FromDotnet<T> (Codec<T> codec, object value) {
        return codec.Parse(DotnetOps.Instance, value).GetResultOrThrow(m => new AssertFailedException(m));
    }

    private static T FromDotnetOrPartial<T>(Codec<T> codec, object value)
    {
        return codec.Parse(DotnetOps.Instance, value).GetResultOrPartialOrThrow(m => new AssertFailedException(m));
    }

    private static string FromDotnetErrorMessage(Codec<string> codec, object value) {
        return codec.Parse(DotnetOps.Instance, value).ErrorResult?.Message ?? throw new AssertFailedException();
    }

    private static void AssertFromDotnetFails<TR>(Codec<TR> codec, object value) {
        DataResult<TR> result = codec.Parse(DotnetOps.Instance, value);
        Assert.IsTrue(result.IsError, "Expected data result error, but got: " + result.GetResultOrThrow());
    }

    private static void AssertFromDotnetFailsPartial<TR>(Codec<TR> codec, object value) {
        DataResult<TR> result = codec.Parse(DotnetOps.Instance, value);
        Assert.IsFalse(result.HasResultOrPartial, "Expected data result error, but got: " + result.GetResultOrPartial());
    }

    private static void AssertToDotnetFails<T>(Codec<T> codec, T value) {
        DataResult<object> result = codec.EncodeStart(DotnetOps.Instance, value);
        Assert.IsTrue(result.IsError, "Expected data result error, but got: " + result.GetResultOrThrow());
    }
    
    private static void AssertRoundTrip<T>(Codec<T> codec, T value, object dotnet) {
        Assert.AreEqual(dotnet, ToDotnet(codec, value), TestComparer);
        Assert.AreEqual(value, FromDotnet(codec, dotnet), TestComparer);
    }

    private static void AssertRoundTrips<T>(IEnumerable<Codec<T>> codecs, T value, object dotnet) {
        foreach (Codec<T> codec in codecs)
        {
            AssertRoundTrip(codec, value, dotnet);
        }
    }
    
    #endregion

    #region Tests

    [TestMethod]
    public void comparer_test()
    {
        Assert.AreEqual(
            new Dictionary<string, int>
            {
                ["foo"] = 1,
                ["bar"] = 2
            },
            JMap.Of([
                "foo", 1, 
                "bar", 2
            ]),
            TestComparer
        );
    }

    [TestMethod]
    public void unboundedDictionary_simple() {
        AssertRoundTrip(
            Codec.CreateUnboundedDictionary(Codec.String, Codec.Int),
            new Dictionary<string, int>
            {
                ["foo"] = 1,
                ["bar"] = 2
            },
            JMap.Of([
                    "foo", 1, 
                    "bar", 2
                ])
        );
    }
    
    [TestMethod]
    public void unboundedDictionary_invalidEntry() {
        Codec<IDictionary<string, int>> codec = Codec.CreateUnboundedDictionary(Codec.String, Codec.Int);
        AssertFromDotnetFails(codec, JMap.Of(
            "foo", 1,
            "bar", "garbage",
            "baz", 3
        ));
    }
    
    [TestMethod]
    public void unboundedMap_invalidEntryPartial() {
        Codec<IDictionary<string, int>> codec = Codec.CreateUnboundedDictionary(Codec.String, Codec.Int);
        Assert.AreEqual(
            new Dictionary<string, int>
            {
                ["foo"] = 1,
                ["baz"] = 3
            },
            FromDotnetOrPartial(codec, JMap.Of(
                "foo", 1,
                "bar", "garbage",
                "baz", 3
            )),
            TestComparer
        );
    }
    
    [TestMethod]
    public void unboundedMap_invalidEntryNestedPartial() {
        Codec<IDictionary<string, IDictionary<string, int>>> codec = Codec.CreateUnboundedDictionary(Codec.String, Codec.CreateUnboundedDictionary(Codec.String, Codec.Int));
        Assert.AreEqual(
            new Dictionary<string, IDictionary<string, int>>
            {
                ["foo"] = new Dictionary<string, int>
                {
                    ["foo"] = 1
                },
                ["bar"] = new Dictionary<string, int>
                {
                    ["foo"] = 1,
                    ["baz"] = 3
                }
            },
            FromDotnetOrPartial(
                codec, JMap.Of(
                    "foo", JMap.Of(
                        "foo", 1
                    ),
                    "bar", JMap.Of(
                        "foo", 1,
                        "bar", "garbage",
                        "baz", 3
                    )
                )
            ),
            TestComparer
        );
    }
    
    [TestMethod]
    public void unboundedMap_repeatedKeys() {
        Codec<IDictionary<string, int>> codec = Codec.CreateUnboundedDictionary(ToLowerCase, Codec.Int);
        AssertFromDotnetFails(codec, new Dictionary<string, int>
        {
            ["foo"] = 1,
            ["FOO"] = 3
        });
    }

    /// <summary>
    /// In <see cref="Hashtable"/>, it insert entry randomly,
    /// but <see cref="T:System.Collections.Generic.Dictionary`2"/> works like queue.
    /// So assign same value to avoid random order.
    /// </summary>
    [TestMethod]
    public void unboundedMap_repeatedKeysPartial() {
        Codec<IDictionary<string, int>> codec = Codec.CreateUnboundedDictionary(ToLowerCase, Codec.Int);
        Assert.AreEqual(
            new Dictionary<string, int>
            {
                // The first entry is picked for the partial result
                ["foo"] = 1,
                ["bar"] = 2
            },
            FromDotnetOrPartial(codec, JMap.Of(
                "foo", 1,
                "bar", 2,
                "FOO", 1
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void list_roundTrip() {
        AssertRoundTrip(
            Codec.String.List(),
            new List<string> { "foo", "bar", "baz" },
            JList.Of("foo", "bar", "baz")
        );
    }
    
    [TestMethod]
    public void list_invalidValues() {
        Codec<IList<string>> codec = Codec.String.List();
        AssertFromDotnetFails(codec, JObjList.Of("foo", 2, "baz", false));

        Assert.AreEqual(
            new List<string> { "foo", "bar" },
            FromDotnetOrPartial(codec, JObjList.Of("foo", "bar", 2, false)),
            TestComparer
        );

        Assert.AreEqual(
            new List<string> { "foo", "baz" },
            FromDotnetOrPartial(codec, JObjList.Of("foo", 2, "baz", false)),
            TestComparer
        );
    }

    [TestMethod]
    public void sizeLimitedList_roundTrip() {
        AssertRoundTrip(
            Codec.String.List(0, 2),
            new List<string> { "foo", "bar" },
            JList.Of("foo", "bar")
        );
    }

    [TestMethod]
    public void sizeLimitedList_tooLong()
    {
        Codec<IList<string>> codec = Codec.String.List(0, 2);
        AssertFromDotnetFails(codec, JObjList.Of("foo", "bar", "baz"));
        AssertToDotnetFails(codec, new List<string> { "foo", "bar", "baz" });

        // Input is clipped in partial result
        Assert.AreEqual(
            new List<string>{ "foo", "bar" },
            FromDotnetOrPartial(codec, JList.Of("foo", "bar", "baz")),
            TestComparer
        );
    }
    
    public void sizeLimitedList_tooLongWithInvalid() {
        Codec<IList<string>> codec = Codec.String.List(0, 2);

        // Input is clipped only by valid entries
        Assert.AreEqual(
            JList.Of("foo", "bar"),
            FromDotnetOrPartial(codec, JObjList.Of("foo", 2, "bar", "baz", false)),
            TestComparer
        );
    }

    [TestMethod]
    public void sizeLimitedList_tooShort() {
        Codec<IList<string>> codec = Codec.String.List(2, 3);
        AssertToDotnetFails(codec, JList.Of("foo"));
        // We can't get any partial result if the data is too short
        AssertFromDotnetFailsPartial(codec, JList.Of("foo"));

        AssertRoundTrip(codec, JList.Of("foo", "bar"), JList.Of("foo", "bar"));
        AssertRoundTrip(codec, JList.Of("foo", "bar", "baz"), JList.Of("foo", "bar", "baz"));
    }

    [TestMethod]
    public void sizeLimitedList_tooShortWithInvalid() {
        Codec<IList<string>> codec = Codec.String.List(2, 3);
        AssertFromDotnetFailsPartial(codec, JObjList.Of("foo", 1, 2));

        Assert.AreEqual(
            JList.Of("foo", "bar"),
            FromDotnetOrPartial(codec, JObjList.Of("foo", 2, "bar", 3)),
            TestComparer
        );
    }

    [TestMethod]
    public void CreateAlternative_simple() {
        Codec<string> codec = Codec.CreateAlternative(Codec.String, Codec.Int, integer => "integer:" + integer);
        AssertRoundTrip(codec, "string", "string");
        Assert.AreEqual("integer:23", FromDotnet(codec, 23), TestComparer);
        // Alternative is only used for reads
        Assert.AreEqual("integer:4", ToDotnet(codec, "integer:4"), TestComparer);

        AssertFromDotnetFails(codec, JMap.Of());
        AssertFromDotnetFails(codec, false);
    }

    public static readonly Codec<string> NeverPrimary = Codec.String.Validate(_ => DataResult.CreateError<string>("Failed Primary"));
    public static readonly Codec<string> NeverAlternative = Codec.String.Validate(_ => DataResult.CreateError<string>("Failed Alternative"));
    public static readonly Codec<string> NeverWithPartialPrimary = Codec.String.Validate(s => DataResult.CreateError("Failed Primary with partial", Optional.Create("Partial Primary: " + s)));
    public static readonly Codec<string> NeverWithPartialAlternative = Codec.String.Validate(s => DataResult.CreateError("Failed Alternative with partial", Optional.Create("Partial Alternative: " + s)));

    [TestMethod]
    public void CreateAlternative_primaryPartialAlternativeFails() {
        Codec<string> codec = Codec.CreateAlternative(
            NeverWithPartialPrimary,
            NeverAlternative
        );
        Assert.AreEqual(
            "Partial Primary: value",
            FromDotnetOrPartial(codec, "value"),
            TestComparer
        );

        Assert.AreEqual(
            "Failed Primary with partial",
            FromDotnetErrorMessage(codec, "value"),
            TestComparer
        );
    }

    [TestMethod]
    public void CreateAlternative_primaryFailsAlternativePartial() {
        Codec<string> codec = Codec.CreateAlternative(
            NeverPrimary,
            NeverWithPartialAlternative
        );
        
        Assert.AreEqual(
            "Partial Alternative: value",
            FromDotnetOrPartial(codec, "value"),
            TestComparer
        );

        Assert.AreEqual(
            "Failed Alternative with partial",
            FromDotnetErrorMessage(codec, "value"),
            TestComparer
        );
    }

    [TestMethod]
    public void CreateAlternative_bothPartialPrefersPrimary() {
        Codec<string> codec = Codec.CreateAlternative(
            NeverWithPartialPrimary,
            NeverWithPartialAlternative
        );
        Assert.AreEqual(
            "Partial Primary: value",
            FromDotnetOrPartial(codec, "value"),
            TestComparer
        );
    }

    /// <summary>
    /// Different message for different implementation.
    /// </summary>
    [TestMethod]
    public void CreateAlternative_bothFail()
    {
        Codec<string> codec = Codec.CreateAlternative(
            NeverPrimary,
            NeverAlternative
        );
        Assert.AreEqual(
            "Failed Alternative;Failed Primary",
            FromDotnetErrorMessage(codec, "value"),
            TestComparer
        );
    }

    [TestMethod]
    public void CreateAlternative_bothSuccessful() {
        Codec<string> codec = Codec.CreateAlternative(Codec.String, ToLowerCase);
        AssertRoundTrip(codec, "string", "string");

        // Primary codec is chosen over alternative
        AssertRoundTrip(codec, "String", "String");
    }

    private record Node(string value, Optional<Node> next) {
        public static readonly Codec<Node> Codec = Serialization.Codecs.Codec.CreateRecursive<Node>("Node", self =>
            RecordCodecBuilder.CreateCodec<Node>(i => i.Group(
                Serialization.Codecs.Codec.String.Field("value").ForGetter<Node>(o => o.value),
                self.OptionalField("next").ForGetter<Node>(o => o.next)
            ).Apply(i, (v, n) => new Node(v, n)))
        );

        public void ToList(IList<string> output) {
            output.Add(value);
            next.IfHasValue(l => l.ToList(output));
        }

        public IList<string> ToList() {
            IList<string> result = new List<string>();
            ToList(result);
            return result;
        }

        private static Optional<Node> Create(IEnumerator<string> values) {
            if (values.MoveNext()) {
                string value = values.Current;
                Optional<Node> next = Create(values);
                return Optional.Create(new Node(value, next));
            }
            return Optional<Node>.Empty;
        }

        public static void AssertParsingEquals(IList<string> asList, object asData) {
            TestDecode(asList, asData);
            TestEncode(asList, asData);
        }

        private static void TestDecode(IList<string> expected, object asData) {
            Assert.AreEqual(expected, FromDotnet(Codec, asData).ToList(), TestComparer);
        }

        private static void TestEncode(IList<string> asList, object expected) {
            Node fromList = Create(asList.GetEnumerator()).GetOrThrow(() => new AssertFailedException());
            Assert.AreEqual(expected, ToDotnet(Codec, fromList), TestComparer);
        }
    }

    [TestMethod]
    public void SelfRecursive() {
        Node.AssertParsingEquals(JList.Of("a"), JMap.Of("value", "a"));
        Node.AssertParsingEquals(JList.Of("a", "b"), JMap.Of("value", "a", "next", JMap.Of("value", "b")));
        Node.AssertParsingEquals(JList.Of("a", "b", "c"), JMap.Of("value", "a", "next", JMap.Of("value", "b", "next", JMap.Of("value", "c"))));
    }

    private record Left(Optional<Right> next) {
        public static readonly Codec<Left> Codec = RecordCodecBuilder.CreateCodec<Left>(i => i.Group(
            Right.Codec.OptionalField("next").ForGetter<Left>(o => o.next)
        ).Apply(i, n => new Left(n)));

        public int Count => 1 + next.Select(r => r.Depth).GetOrDefault(0);

        public static Optional<Left> Create(int length) {
            return length == 0 ? Optional<Left>.Empty : Optional.Create(new Left(Right.Create(length - 1)));
        }
    }

    private record Right(Optional<Left> next) {
        public static readonly Codec<Right> Codec = Serialization.Codecs.Codec.CreateRecursive<Right>("Right", _ =>
            RecordCodecBuilder.CreateCodec<Right>(i => i.Group(
                Left.Codec.OptionalField("next").ForGetter<Right>(o => o.next)
            ).Apply(i, n => new Right(n)))
        );

        public int Depth => 1 + next.Select(l => l.Count).GetOrDefault(0);

        public static Optional<Right> Create(int depth) {
            return depth == 0 ? Optional<Right>.Empty : Optional.Create(new Right(Left.Create(depth - 1)));
        }

        public static IDictionary<string, object> CreateChain(int depth) {
            return depth == 1 ? JMap.Of<string, object>() : JMap.Of<string, object>("next", CreateChain(depth - 1));
        }

        public static void AssertParsingAtDepth(int depth) {
            IDictionary<string, object> asData = CreateChain(depth);
            TestDecode(depth, asData);
            TestEncode(depth, asData);
        }

        private static void TestDecode(int depth, IDictionary<string, object> asData) {
            Right parsed = FromDotnet(Codec, asData);
            Assert.AreEqual(depth, parsed.Depth, TestComparer);
        }

        private static void TestEncode(int depth, IDictionary<string, object> asData) {
            Right fresh = Create(depth).GetOrThrow(() => new AssertFailedException());
            Assert.AreEqual(asData, ToDotnet(Codec, fresh), TestComparer);
        }
    }

    [TestMethod]
    public void MutuallyRecursiveCodecTest() {
        Right.AssertParsingAtDepth(1);
        Right.AssertParsingAtDepth(2);
        Right.AssertParsingAtDepth(3);
    }

    private class Variant {
        public static readonly Variant Foo = new(V.Foo);
        public static readonly Variant Bar = new(V.Bar);

        public static readonly Codec<Variant> Codec = Serialization.Codecs.Codec.CreateStringResolver(
            variant => variant._var switch
            {
                V.Foo => "foo",
                V.Bar => "bar",
                _ => null
            },
            str => str switch
            {
                "foo" => Foo,
                "bar" => Bar,
                _ => null,
            }
        );

        private Variant(V var)
        {
            _var = var;
        }
        
        private readonly V _var;
        
        private enum V
        {
            Foo,
            Bar
        }
    }

    [TestMethod]
    public void stringResolver_simple() {
        AssertRoundTrip(Variant.Codec, Variant.Foo, "foo");
        AssertRoundTrip(Variant.Codec, Variant.Bar, "bar");
        AssertFromDotnetFails(Variant.Codec, "baz");
    }

    private record DispatchType {
        public static readonly DispatchType Any = new(DType.Any, "any", Serialization.Codecs.Codec.String);
        public static readonly DispatchType LowerCase = new(DType.LowerCase,"lower_case", Serialization.Codecs.Codec.String.Validate(s => s.ToLowerInvariant().Equals(s, StringComparison.Ordinal) ? DataResult.CreateSuccess(s) : DataResult.CreateError<string>("Not lower case: " + s)));
        public static readonly DispatchType UpperCase = new(DType.UpperCase,"upper_case", Serialization.Codecs.Codec.String.Validate(s => s.ToUpperInvariant().Equals(s, StringComparison.Ordinal) ? DataResult.CreateSuccess(s) : DataResult.CreateError<string>("Not upper case: " + s)));
        public static readonly DispatchType Never = new(DType.Never,"never", Serialization.Codecs.Codec.String.Validate(_ => DataResult.CreateError<string>("No")));
        public static readonly DispatchType NeverWithPartial = new(DType.NeverWithPartial,"never_with_partial", Serialization.Codecs.Codec.String.Validate(s => DataResult.CreateError("No", Optional.Create(s))));

        public static readonly Codec<DispatchType> Codec = Serialization.Codecs.Codec.CreateStringResolver(t => t.GetSerializedName(), str => Lookup(str));
        public static readonly Codec<DispatchType> CaseInsensitiveCodec = Serialization.Codecs.Codec.CreateStringResolver(t => t.GetSerializedName(), str => Lookup(str, true));

        public readonly DType Type;
        private readonly string _name;
        public readonly Codec<string> ElemCodec;
        

        DispatchType(DType dType, string name, Codec<string> elemCodec) {
            Type = dType;
            _name = name;
            ElemCodec = elemCodec;
        }

        private static readonly DispatchType[] Values = [Any, LowerCase, UpperCase, Never, NeverWithPartial];

        private static DispatchType Lookup(string name, bool ignoreCase = false)
        {
            StringComparison c = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return Values.FirstOrDefault(type => type.GetSerializedName().Equals(name, c));
        }

        public string GetSerializedName() {
            return _name;
        }
        
        public enum DType
        {
            Any,
            LowerCase,
            UpperCase,
            Never,
            NeverWithPartial
        }
    }

    private static readonly Codec<IDictionary<DispatchType, string>> DispatchedMapCodec = Codec.CreateDispatchedDictionary(DispatchType.Codec, t => t.ElemCodec);

    [TestMethod]
    public void dispatchedMap_encode() {
        Assert.AreEqual(
            JMap.Of(
                "any", "Some text",
                "lower_case", "very quietly",
                "upper_case", "NOT SHOUTING"
            ),
            ToDotnet(DispatchedMapCodec, JMap.Of(
                DispatchType.Any, "Some text",
                DispatchType.LowerCase, "very quietly",
                DispatchType.UpperCase, "NOT SHOUTING"
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void dispatchedMap_decode() {
        Assert.AreEqual(
            JMap.Of(
                DispatchType.Any, "Some text",
                DispatchType.LowerCase, "very quietly",
                DispatchType.UpperCase, "NOT SHOUTING"
            ),
            FromDotnet(DispatchedMapCodec, JMap.Of(
                "any", "Some text",
                "lower_case", "very quietly",
                "upper_case", "NOT SHOUTING"
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void dispatchedMap_decodeInvalidType() {
        AssertFromDotnetFails(DispatchedMapCodec, JMap.Of(
            "invalid", "Some text"
        ));
    }

    [TestMethod]
    public void dispatchedMap_decodeInvalidValue() {
        AssertFromDotnetFails(DispatchedMapCodec, JMap.Of(
            "lower_case", "SHOUTING"
        ));
    }

    [TestMethod]
    public void dispatchedMap_decodePartialResult() {
        Assert.AreEqual(
            JMap.Of(
                DispatchType.Any, "Some text",
                DispatchType.UpperCase, "NOT SHOUTING"
            ),
            FromDotnetOrPartial(DispatchedMapCodec, JMap.Of(
                "any", "Some text",
                "invalid", string.Empty,
                "lower_case", "SHOUTING",
                "upper_case", "NOT SHOUTING"
            )),
            TestComparer
        );

        Assert.AreEqual(
            JMap.Of(
                DispatchType.Any, "Some text",
                DispatchType.UpperCase, "NOT SHOUTING"
            ),
            FromDotnetOrPartial(DispatchedMapCodec, JMap.Of(
                "invalid", string.Empty,
                "any", "Some text",
                "upper_case", "NOT SHOUTING"
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void dispatchedMap_decodeNestedPartialResult()
    {
        Assert.AreEqual(
            JMap.Of(
                DispatchType.NeverWithPartial, "Fails with partial result",
                DispatchType.Any, "Something else"
            ),
            FromDotnetOrPartial(DispatchedMapCodec, JMap.Of(
                "never_with_partial", "Fails with partial result",
                "any", "Something else"
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void dispatchedMap_decodeRepeatedEntries() {
        Codec<IDictionary<DispatchType, string>> dispatchedMapCodec = Codec.CreateDispatchedDictionary(DispatchType.CaseInsensitiveCodec, t => t.ElemCodec);

        AssertFromDotnetFails(dispatchedMapCodec, JMap.Of(
            "lower_case", "first",
            "LOWER_CASE", "second"
        ));

        Assert.AreEqual(
            JMap.Of(
                DispatchType.LowerCase, "first"
            ),
            FromDotnetOrPartial(dispatchedMapCodec, JMap.Of(
                "lower_case", "first",
                "LOWER_CASE", "first"
            )),
            TestComparer
        );
    }

    private record SimpleOptionals(
        Optional<string> str,
        Optional<int> integer
    ) {
        public static readonly Codec<SimpleOptionals> StrictCodec = RecordCodecBuilder.CreateCodec<SimpleOptionals>(i => i.Group(
            Codec.String.OptionalField("string").ForGetter<SimpleOptionals>(so => so.str),
            Codec.Int.OptionalField("integer").ForGetter<SimpleOptionals>(so => so.integer)
        ).Apply(i, (s, oi) => new SimpleOptionals(s, oi)));

        public static readonly Codec<SimpleOptionals> LenientCodec = RecordCodecBuilder.CreateCodec<SimpleOptionals>(i => i.Group(
            Codec.String.OptionalField("string", true).ForGetter<SimpleOptionals>(so => so.str),
            Codec.Int.OptionalField("integer", true).ForGetter<SimpleOptionals>(so => so.integer)
        ).Apply(i, (s, oi) => new SimpleOptionals(s, oi)));
    }

    [TestMethod]
    public void optionalField_roundTrip() {
        AssertRoundTrips(
            JList.Of(SimpleOptionals.StrictCodec, SimpleOptionals.LenientCodec),
            new SimpleOptionals(Optional.Create("foo"), Optional.Create(1)),
            JMap.Of(
                "string", "foo",
                "integer", 1
            )
        );
        AssertRoundTrips(
            JList.Of(SimpleOptionals.StrictCodec, SimpleOptionals.LenientCodec),
            new SimpleOptionals(Optional<string>.Empty, Optional.Create(1)),
            JMap.Of(
                "integer", 1
            )
        );
    }

    [TestMethod]
    public void optionalField_strictInvalidValues() {
        AssertFromDotnetFails(
            SimpleOptionals.StrictCodec,
            JMap.Of("string", 54)
        );
        AssertFromDotnetFails(
            SimpleOptionals.StrictCodec,
            JMap.Of("integer", "not an int")
        );
    }

    [TestMethod]
    public void optionalField_strictInvalidValuesPartial() {
        Assert.AreEqual(
            new SimpleOptionals(Optional<string>.Empty, Optional.Create(23)),
            FromDotnetOrPartial(SimpleOptionals.StrictCodec, JMap.Of(
                "string", false,
                "integer", 23
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void optionalField_lenientInvalidValues() {
        Assert.AreEqual(
            new SimpleOptionals(Optional<string>.Empty, Optional.Create(23)),
            FromDotnet(SimpleOptionals.LenientCodec, JMap.Of(
                "string", false,
                "integer", 23
            ))
        );
    }

    private record NestedStrictOptionals(
        Optional<SimpleOptionals> nested
    ) {
        public static readonly Codec<NestedStrictOptionals> TopLevelStrictCodec = RecordCodecBuilder.CreateCodec<NestedStrictOptionals>(i => i.Group(
            SimpleOptionals.StrictCodec.OptionalField("nested").ForGetter<NestedStrictOptionals>(n => n.nested)
        ).Apply(i, n => new NestedStrictOptionals(n)));

        public static readonly Codec<NestedStrictOptionals> TopLevelLenientCodec = RecordCodecBuilder.CreateCodec<NestedStrictOptionals>(i => i.Group(
            SimpleOptionals.StrictCodec.OptionalField("nested", true).ForGetter<NestedStrictOptionals>(n => n.nested)
        ).Apply(i, n => new NestedStrictOptionals(n)));
    }

    [TestMethod]
    public void optionalField_nestedStrictOptionals() {
        Assert.AreEqual(
            new NestedStrictOptionals(
                Optional.Create(new SimpleOptionals(
                    Optional.Create("foo"),
                    Optional.Create(1)
                ))
            ),
            FromDotnet(NestedStrictOptionals.TopLevelStrictCodec, JMap.Of(
                "nested", JMap.Of(
                    "string", "foo",
                    "integer", 1
                )
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void optionalField_nestedStrictOptionalsPartialResult()
    {
        Assert.AreEqual(
            new NestedStrictOptionals(
                Optional.Create(new SimpleOptionals(
                    Optional.Create("foo"),
                    Optional<int>.Empty
                ))
            ),
            FromDotnetOrPartial(
                NestedStrictOptionals.TopLevelStrictCodec, JMap.Of(
                    "nested", JMap.Of(
                        "string", "foo",
                        "integer", "not an int"
                    )
                )
            ),
            TestComparer
        );

        Assert.AreEqual(
            new NestedStrictOptionals(
                Optional<SimpleOptionals>.Empty
            ),
            FromDotnet(NestedStrictOptionals.TopLevelLenientCodec, JMap.Of(
                "nested", JMap.Of(
                    "string", "foo",
                    "integer", "not an int"
                )
            )),
            TestComparer
        );
    }

    private record Simple(string str, int integer) {
        public static readonly Codec<Simple> Codec = RecordCodecBuilder.CreateCodec<Simple>(i => i.Group(
            Serialization.Codecs.Codec.String.Field("string").ForGetter<Simple>(s => s.str),
            Serialization.Codecs.Codec.Int.Field("integer").ForGetter<Simple>(s => s.integer)
        ).Apply(i, (s, oi) => new Simple(s, oi)));
    }

    [TestMethod]
    public void assumeMap_recordCodec() {
        AssertRoundTrips(
            JList.Of(
                Simple.Codec,
                // Wrapped directly
                MapCodec.AssumeMapUnsafe(Simple.Codec).AsCodec(),
                // From a non-MapCodecCodec
                MapCodec.AssumeMapUnsafe(ObfuscateCodecType(Simple.Codec)).AsCodec()
            ),
            new Simple("hello", 1),
            JMap.Of(
                "string", "hello",
                "integer", 1
            )
        );

        AssertFromDotnetFails(
            MapCodec.AssumeMapUnsafe(Simple.Codec).AsCodec(),
            "not a map"
        );
    }

    private static Codec<TA> ObfuscateCodecType<TA>(Codec<TA> codec) {
        return Codec.Create(codec, codec);
    }

    /// <summary>
    /// With tracked codec backend, transform between codec and map codec will not be boxed recursively.
    /// So this test performs different from mojang's
    /// </summary>
    [TestMethod]
    public void assumeMap_primitiveCodec() {
        // This codec should be original Codec.Int
        Codec<int> codec = MapCodec.AssumeMapUnsafe(Codec.Int).AsCodec();
        AssertRoundTrip(codec, 123, 123);
        AssertRoundTrip(codec, 123, 123);
    }

    private record RecordWith5Fields(int f1, int f2, int f3, int f4, int f5) {
        public static readonly Codec<RecordWith5Fields> Codec = RecordCodecBuilder.CreateCodec<RecordWith5Fields>(i => i.Group(
            Serialization.Codecs.Codec.Int.Field("f1").ForGetter<RecordWith5Fields>(r => r.f1),
            Serialization.Codecs.Codec.Int.Field("f2").ForGetter<RecordWith5Fields>(r => r.f2),
            Serialization.Codecs.Codec.Int.Field("f3").ForGetter<RecordWith5Fields>(r => r.f3),
            Serialization.Codecs.Codec.Int.Field("f4").ForGetter<RecordWith5Fields>(r => r.f4),
            Serialization.Codecs.Codec.Int.Field("f5").ForGetter<RecordWith5Fields>(r => r.f5)
        ).Apply(i, (f1, f2, f3, f4, f5) => new RecordWith5Fields(f1, f2, f3, f4, f5)));
    }

    private record RecordWith7Fields(int f1, int f2, int f3, int f4, int f5, int f6, int f7) {
        public static readonly Codec<RecordWith7Fields> Codec = RecordCodecBuilder.CreateCodec<RecordWith7Fields>(i => i.Group(
            Serialization.Codecs.Codec.Int.Field("f1").ForGetter<RecordWith7Fields>(r => r.f1),
            Serialization.Codecs.Codec.Int.Field("f2").ForGetter<RecordWith7Fields>(r => r.f2),
            Serialization.Codecs.Codec.Int.Field("f3").ForGetter<RecordWith7Fields>(r => r.f3),
            Serialization.Codecs.Codec.Int.Field("f4").ForGetter<RecordWith7Fields>(r => r.f4),
            Serialization.Codecs.Codec.Int.Field("f5").ForGetter<RecordWith7Fields>(r => r.f5),
            Serialization.Codecs.Codec.Int.Field("f6").ForGetter<RecordWith7Fields>(r => r.f6),
            Serialization.Codecs.Codec.Int.Field("f7").ForGetter<RecordWith7Fields>(r => r.f7)
        ).Apply(i, (f1, f2, f3, f4, f5, f6, f7) => new RecordWith7Fields(f1, f2, f3, f4, f5, f6, f7)));
    }

    private static void AssertMapOrderEqual(IDictionary expected, object actual) {
        Assert.IsTrue(actual is IDictionary);
        object[] es1 = expected.Cast<object>().ToArray();
        object[] es2 = ((IDictionary) actual).Cast<object>().ToArray();
        Assert.IsTrue(es1.SequenceEqual(es2));
    }

    [TestMethod]
    public void recordCodec_maintainFieldOrder() {
        AssertMapOrderEqual(
            (IDictionary) JMap.Of(
                "f1", 5,
                "f2", 4,
                "f3", 3,
                "f4", 2,
                "f5", 1
            ),
            ToDotnet(RecordWith5Fields.Codec, new RecordWith5Fields(5, 4, 3, 2, 1))
        );

        AssertMapOrderEqual(
            (IDictionary) JMap.Of(
                "f1", 7,
                "f2", 6,
                "f3", 5,
                "f4", 4,
                "f5", 3,
                "f6", 2,
                "f7", 1
            ),
            ToDotnet(RecordWith7Fields.Codec, new RecordWith7Fields(7, 6, 5, 4, 3, 2, 1))
        );
    }

    private record DispatchedValue(DispatchType type, string value) {
        public static readonly Codec<DispatchedValue> Codec = DispatchType.Codec.Dispatch(v => v.type, t =>
            t.ElemCodec.Field("value").XMap(s => new DispatchedValue(t, s), v => v.value)
        );
    }

    [TestMethod]
    public void valueDispatch_Normal()
    {
        AssertRoundTrip(
            DispatchedValue.Codec.List(),
            JList.Of(
                new DispatchedValue(DispatchType.Any, "Some text"),
                new DispatchedValue(DispatchType.LowerCase, "very quietly"),
                new DispatchedValue(DispatchType.UpperCase, "NOT SHOUTING")
            ),
            JList.Of(
                JMap.Of("type", "any", "value", "Some text"),
                JMap.Of("type", "lower_case", "value", "very quietly"),
                JMap.Of("type", "upper_case", "value", "NOT SHOUTING")
            )
        );
    }

    [TestMethod]
    public void valueDispatch_decodeInvalidType() {
        AssertFromDotnetFails(DispatchedValue.Codec, JMap.Of(
            "type", "invalid",
            "value", "Some text"
        ));
    }

    [TestMethod]
    public void valueDispatch_decodeMissingType() {
        AssertFromDotnetFails(DispatchedValue.Codec, JMap.Of(
            "value", "Some text"
        ));
    }

    [TestMethod]
    public void valueDispatch_decodeInvalidValue() {
        AssertFromDotnetFails(DispatchedValue.Codec, JMap.Of(
            "type", "lower_case",
            "value", "SHOUTING"
        ));
    }

    [TestMethod]
    public void valueDispatch_decodeInvalidValuePartialResult() {
        Assert.AreEqual(
            new DispatchedValue(DispatchType.NeverWithPartial, "Some text"),
            FromDotnetOrPartial(DispatchedValue.Codec, JMap.Of(
                "type", "never_with_partial",
                "value", "Some text"
            )),
            TestComparer
        );
    }

    [TestMethod]
    public void unitMapCodec_Encoding() {
        object marker = new object();

        AssertRoundTrip(
            MapCodec.CreateUnit(marker).AsCodec(),
            marker,
            JMap.Of()
        );
    }

    #endregion
}