using System.Buffers;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataFixerUpper.Datafixers.Kinds;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.Codecs.Builder;
using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

//#nullable disable

namespace DataFixerUpper.Test;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        string jsonString = "[1,2,3,4,5]";
        JsonNode json = JsonNode.Parse(jsonString)!;
        Codec<IList<int>> intListCodec = Codec.Int.List(0, 100);
        DataResult<IList<int>> result = intListCodec.Parse(JsonOps.Instance, json);
        result.IfSuccess(l => Console.WriteLine(l.Sum()));
    }
}

public class SomeRecord {
    public int IntValue { get; }
    public string StringValue { get; }
    public IList<string> StringValues { get; }
    public SomeRecord(int i, string s, IList<string> ls) {
        IntValue = i;
        StringValue = s;
        StringValues = ls;
    }
    // methods elided
}
