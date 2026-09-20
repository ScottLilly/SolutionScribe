using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolutionScribe.Core.Json;
using System;
using System.Collections.Generic;

namespace Tests.SolutionScribe.Core;

[TestClass]
public class FlatJsonTests
{
    #region Writing

    [TestMethod]
    public void Write_NoPairs_WritesAnEmptyObject()
    {
        Assert.AreEqual("{}", FlatJson.Write(new Dictionary<string, string>()));
    }

    [TestMethod]
    public void Write_OnePair_WritesItIndentedOnItsOwnLine()
    {
        string json = FlatJson.Write(new Dictionary<string, string> { ["Name"] = "Scott Lilly" });

        Assert.AreEqual(
            "{" + Environment.NewLine +
            "  \"Name\": \"Scott Lilly\"" + Environment.NewLine +
            "}",
            json);
    }

    [TestMethod]
    public void Write_TwoPairs_SeparatesThemWithACommaAndANewLine()
    {
        string json = FlatJson.Write(new Dictionary<string, string>
        {
            ["First"] = "1",
            ["Second"] = "2"
        });

        Assert.AreEqual(
            "{" + Environment.NewLine +
            "  \"First\": \"1\"," + Environment.NewLine +
            "  \"Second\": \"2\"" + Environment.NewLine +
            "}",
            json);
    }

    [TestMethod]
    public void Write_ValueHoldingQuotesAndBackslashes_EscapesThem()
    {
        string json = FlatJson.Write(new Dictionary<string, string>
        {
            ["Key"] = @"He said ""hi"" in C:\Temp"
        });

        StringAssert.Contains(json, @"""He said \""hi\"" in C:\\Temp""");
    }

    [TestMethod]
    public void Write_ValueHoldingControlCharacters_EscapesThem()
    {
        string json = FlatJson.Write(new Dictionary<string, string>
        {
            ["Key"] = "line\r\nnext\ttabbed\u0001"
        });

        StringAssert.Contains(json, @"""line\r\nnext\ttabbed\u0001""");
    }

    [TestMethod]
    public void Write_NonAsciiValue_WritesTheCharactersThemselves()
    {
        // The file is written and read as UTF-8 at both ends, so escaping these would only make
        // the file harder to read.
        string json = FlatJson.Write(new Dictionary<string, string> { ["Key"] = "Jorge Muller \u00e9" });

        StringAssert.Contains(json, "\"Jorge Muller \u00e9\"");
    }

    #endregion

    #region Parsing

    [TestMethod]
    public void Parse_AnEmptyObject_ReturnsNoPairs()
    {
        Assert.AreEqual(0, FlatJson.Parse("{}").Count);
    }

    [TestMethod]
    public void Parse_OnePair_ReturnsIt()
    {
        var values = FlatJson.Parse(@"{ ""Name"": ""Scott Lilly"" }");

        Assert.AreEqual(1, values.Count);
        Assert.AreEqual("Scott Lilly", values["Name"]);
    }

    [TestMethod]
    public void Parse_WhitespaceAroundEveryToken_ReturnsThePairs()
    {
        var values = FlatJson.Parse("\r\n\t {\r\n\t \"First\"\t : \r\n \"1\" \t,\r\n \"Second\" : \"2\"\r\n }\r\n\t ");

        Assert.AreEqual("1", values["First"]);
        Assert.AreEqual("2", values["Second"]);
    }

    [TestMethod]
    public void Parse_EscapedCharacters_UnescapesThem()
    {
        var values = FlatJson.Parse(@"{ ""Key"": ""quote \"" solidus \/ backslash \\ \b\f\n\r\t"" }");

        Assert.AreEqual("quote \" solidus / backslash \\ \b\f\n\r\t", values["Key"]);
    }

    [TestMethod]
    public void Parse_UnicodeEscapes_UnescapesThemInEitherCase()
    {
        var values = FlatJson.Parse(@"{ ""Key"": ""\u00e9 \u00E9 \u0041"" }");

        Assert.AreEqual("\u00e9 \u00e9 A", values["Key"]);
    }

    [TestMethod]
    public void Parse_AnEscapedKey_UnescapesIt()
    {
        var values = FlatJson.Parse(@"{ ""a\\b"": ""value"" }");

        Assert.AreEqual("value", values[@"a\b"]);
    }

    [TestMethod]
    public void Parse_TheSameKeyTwice_KeepsTheLastValue()
    {
        var values = FlatJson.Parse(@"{ ""Key"": ""first"", ""Key"": ""second"" }");

        Assert.AreEqual(1, values.Count);
        Assert.AreEqual("second", values["Key"]);
    }

    [TestMethod]
    public void Parse_AnEmptyStringValue_ReturnsIt()
    {
        Assert.AreEqual(string.Empty, FlatJson.Parse(@"{ ""Key"": """" }")["Key"]);
    }

    #endregion

    #region Input that is not a flat object of strings

    [TestMethod]
    public void Parse_TextThatIsNotJsonAtAll_ThrowsFormatException()
    {
        AssertThrowsFormatException("{ this is not json");
    }

    [TestMethod]
    public void Parse_AnArray_ThrowsFormatException()
    {
        AssertThrowsFormatException("[ 1, 2, 3 ]");
    }

    [TestMethod]
    public void Parse_AnEmptyDocument_ThrowsFormatException()
    {
        AssertThrowsFormatException(string.Empty);
    }

    [TestMethod]
    public void Parse_AValueThatIsNotAString_ThrowsFormatException()
    {
        // A JSON library would convert these. Nothing writes them, so a file holding one has been
        // edited into a shape this reader was never told about, and guessing is worse than saying
        // so.
        AssertThrowsFormatException(@"{ ""Key"": 1 }");
        AssertThrowsFormatException(@"{ ""Key"": true }");
        AssertThrowsFormatException(@"{ ""Key"": null }");
        AssertThrowsFormatException(@"{ ""Key"": { ""Nested"": ""value"" } }");
        AssertThrowsFormatException(@"{ ""Key"": [ ""value"" ] }");
    }

    [TestMethod]
    public void Parse_AnUnquotedKey_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ Key: ""value"" }");
    }

    [TestMethod]
    public void Parse_ATrailingComma_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""value"", }");
    }

    [TestMethod]
    public void Parse_AMissingComma_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""First"": ""1"" ""Second"": ""2"" }");
    }

    [TestMethod]
    public void Parse_AnUnterminatedString_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""value }");
    }

    [TestMethod]
    public void Parse_AnUnclosedObject_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""value""");
    }

    [TestMethod]
    public void Parse_ContentAfterTheObject_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""value"" } and then some");
    }

    [TestMethod]
    public void Parse_AnUnescapedControlCharacterInAString_ThrowsFormatException()
    {
        AssertThrowsFormatException("{ \"Key\": \"one\r\ntwo\" }");
    }

    [TestMethod]
    public void Parse_AnEscapeThatIsNotAnEscape_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""\q"" }");
    }

    [TestMethod]
    public void Parse_AUnicodeEscapeWithoutFourHexDigits_ThrowsFormatException()
    {
        AssertThrowsFormatException(@"{ ""Key"": ""\u00g1"" }");
        AssertThrowsFormatException(@"{ ""Key"": ""\u12"" }");
        AssertThrowsFormatException(@"{ ""Key"": ""\u 123"" }");
    }

    #endregion

    #region Round trips

    [TestMethod]
    public void Parse_OutputOfWrite_ReturnsTheSameValues()
    {
        var original = new Dictionary<string, string>
        {
            ["Plain"] = "Scott Lilly",
            ["Punctuated"] = "He said \"hi\" in C:\\Temp",
            ["Controls"] = "line\r\nnext\ttabbed\u0001",
            ["NonAscii"] = "\u00e9\u00fc\u4e2d",
            ["Empty"] = string.Empty
        };

        var roundTripped = FlatJson.Parse(FlatJson.Write(original));

        CollectionAssert.AreEquivalent(original, roundTripped);
    }

    [TestMethod]
    public void Parse_AFileWrittenByTheNewtonsoftVersion_ReturnsTheValues()
    {
        // The format Newtonsoft.Json wrote with Formatting.Indented, which is what is sitting in
        // %AppData% for anyone who used an earlier build. Losing it would silently forget the
        // copyright holder they saved.
        string json =
            "{\r\n  \"DefaultCopyrightHolder\": \"Scott Lilly\"\r\n}";

        Assert.AreEqual("Scott Lilly", FlatJson.Parse(json)["DefaultCopyrightHolder"]);
    }

    #endregion

    private static void AssertThrowsFormatException(string json)
    {
        Assert.Throws<FormatException>(
            () => FlatJson.Parse(json),
            $"Parsing '{json}' should have been rejected.");
    }
}
