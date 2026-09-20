using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SolutionScribe.Core.Json;

/// <summary>
/// Reads and writes the one JSON shape this project stores: an object whose every value is a
/// string, such as <c>{ "DefaultCopyrightHolder": "Scott Lilly" }</c>.
/// </summary>
/// <remarks>
/// Hand-written so <c>SolutionScribe.Core</c> needs no package references. A JSON library would
/// put a second copy of an assembly Visual Studio already loads into the extension's probing path,
/// which is a version conflict waiting to happen in a process whose assembly loading the extension
/// does not control.
/// </remarks>
public static class FlatJson
{
    /// <summary>
    /// Writes the pairs as an indented JSON object. An empty collection gives <c>{}</c>.
    /// </summary>
    public static string Write(IReadOnlyDictionary<string, string> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (values.Count == 0)
        {
            return "{}";
        }

        var json = new StringBuilder("{");
        bool isFirst = true;

        foreach (var pair in values)
        {
            if (!isFirst)
            {
                json.Append(',');
            }

            isFirst = false;

            json.Append(Environment.NewLine).Append("  ");
            AppendString(json, pair.Key);
            json.Append(": ");
            AppendString(json, pair.Value);
        }

        return json.Append(Environment.NewLine).Append('}').ToString();
    }

    /// <summary>
    /// Reads a JSON object whose values are all strings. Where a key appears twice, the last one
    /// wins.
    /// </summary>
    /// <exception cref="FormatException">
    /// The text is not a JSON object, or holds a value that is not a string. A number, a boolean,
    /// a null, a nested object and an array are all rejected rather than converted, because
    /// nothing writes them and a file holding one has been edited into a shape its reader was
    /// never told about.
    /// </exception>
    public static Dictionary<string, string> Parse(string json)
    {
        if (json == null)
        {
            throw new ArgumentNullException(nameof(json));
        }

        var values = new Dictionary<string, string>();
        int index = 0;

        SkipWhitespace(json, ref index);
        Expect(json, ref index, '{');
        SkipWhitespace(json, ref index);

        if (index < json.Length && json[index] == '}')
        {
            index++;
        }
        else
        {
            while (true)
            {
                SkipWhitespace(json, ref index);
                string key = ReadString(json, ref index);

                SkipWhitespace(json, ref index);
                Expect(json, ref index, ':');

                SkipWhitespace(json, ref index);
                values[key] = ReadString(json, ref index);

                SkipWhitespace(json, ref index);
                char delimiter = Read(json, ref index);

                if (delimiter == '}')
                {
                    break;
                }

                if (delimiter != ',')
                {
                    throw Invalid(index - 1, "expected ',' or '}'");
                }
            }
        }

        SkipWhitespace(json, ref index);

        if (index != json.Length)
        {
            throw Invalid(index, "expected the end of the document");
        }

        return values;
    }

    private static void AppendString(StringBuilder json, string value)
    {
        json.Append('"');

        foreach (char character in value)
        {
            switch (character)
            {
                case '"':
                    json.Append("\\\"");
                    break;
                case '\\':
                    json.Append("\\\\");
                    break;
                case '\b':
                    json.Append("\\b");
                    break;
                case '\f':
                    json.Append("\\f");
                    break;
                case '\n':
                    json.Append("\\n");
                    break;
                case '\r':
                    json.Append("\\r");
                    break;
                case '\t':
                    json.Append("\\t");
                    break;
                default:
                    if (character < ' ')
                    {
                        json.Append("\\u")
                            .Append(((int)character).ToString("x4", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        // Everything above the control characters is written as itself, including
                        // non-ASCII, because the file is read and written as UTF-8 either way.
                        json.Append(character);
                    }

                    break;
            }
        }

        json.Append('"');
    }

    private static string ReadString(string json, ref int index)
    {
        Expect(json, ref index, '"');

        var value = new StringBuilder();

        while (true)
        {
            char character = Read(json, ref index);

            if (character == '"')
            {
                return value.ToString();
            }

            if (character < ' ')
            {
                throw Invalid(index - 1, "a string cannot hold an unescaped control character");
            }

            if (character != '\\')
            {
                value.Append(character);

                continue;
            }

            char escape = Read(json, ref index);

            switch (escape)
            {
                case '"':
                    value.Append('"');
                    break;
                case '\\':
                    value.Append('\\');
                    break;
                case '/':
                    value.Append('/');
                    break;
                case 'b':
                    value.Append('\b');
                    break;
                case 'f':
                    value.Append('\f');
                    break;
                case 'n':
                    value.Append('\n');
                    break;
                case 'r':
                    value.Append('\r');
                    break;
                case 't':
                    value.Append('\t');
                    break;
                case 'u':
                    value.Append(ReadUnicodeEscape(json, ref index));
                    break;
                default:
                    throw Invalid(index - 1, $"'\\{escape}' is not a valid escape");
            }
        }
    }

    private static char ReadUnicodeEscape(string json, ref int index)
    {
        int value = 0;

        for (int digit = 0; digit < 4; digit++)
        {
            int nibble = HexValue(Read(json, ref index));

            if (nibble < 0)
            {
                throw Invalid(index - 1, @"a \u escape needs four hexadecimal digits");
            }

            value = (value << 4) + nibble;
        }

        return (char)value;
    }

    private static int HexValue(char character)
    {
        if (character >= '0' && character <= '9')
        {
            return character - '0';
        }

        if (character >= 'a' && character <= 'f')
        {
            return character - 'a' + 10;
        }

        if (character >= 'A' && character <= 'F')
        {
            return character - 'A' + 10;
        }

        return -1;
    }

    private static void SkipWhitespace(string json, ref int index)
    {
        while (index < json.Length &&
               (json[index] == ' ' || json[index] == '\t' || json[index] == '\r' || json[index] == '\n'))
        {
            index++;
        }
    }

    private static char Read(string json, ref int index)
    {
        if (index >= json.Length)
        {
            throw Invalid(index, "the document ended before it was complete");
        }

        return json[index++];
    }

    private static void Expect(string json, ref int index, char expected)
    {
        if (Read(json, ref index) != expected)
        {
            throw Invalid(index - 1, $"expected '{expected}'");
        }
    }

    private static FormatException Invalid(int index, string message) =>
        new FormatException($"Invalid JSON at position {index}: {message}.");
}
