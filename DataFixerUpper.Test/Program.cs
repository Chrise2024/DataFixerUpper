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
        Console.WriteLine(Optional.Create(default(int?)).GetType().FullName);
        //F(default(int?));
    }

    static int Add(int a, int b)
    {
        return a + b;
    }

    static T? F<T>(T? v) where T : notnull
    {
        Console.WriteLine(typeof(Optional<T>).FullName);
        Console.WriteLine(typeof(T).FullName);
        Console.WriteLine(v is null);
        return v;
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
