using System.Text;

namespace DoorstepCO.Api.Batch;

/// <summary>
/// Reads and writes delimited text as described by RFC 4180: fields may be quoted, and quoted fields may contain
/// delimiters, doubled quotes and line breaks.
/// </summary>
internal static class CsvFormat
{
    private const char Quote = '"';
    private const char CarriageReturn = '\r';
    private const char LineFeed = '\n';
    private const string LineBreak = "\r\n";

    private static readonly char[] _candidateDelimiters = [',', ';', '\t'];

    public static char DetectDelimiter(string text)
    {
        var counts = new int[_candidateDelimiters.Length];
        var inQuotes = false;
        foreach (var c in text)
        {
            if (c == Quote)
            {
                inQuotes = !inQuotes;
            }
            else if (!inQuotes && (c == CarriageReturn || c == LineFeed))
            {
                break;
            }
            else if (!inQuotes && Array.IndexOf(_candidateDelimiters, c) is var index and >= 0)
            {
                counts[index]++;
            }
        }

        var best = 0;
        for (var i = 1; i < counts.Length; i++)
        {
            if (counts[i] > counts[best])
            {
                best = i;
            }
        }

        return _candidateDelimiters[best];
    }

    public static IReadOnlyList<IReadOnlyList<string>> Parse(string text, char delimiter)
    {
        var rows = new List<IReadOnlyList<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (inQuotes)
            {
                if (c != Quote)
                {
                    field.Append(c);
                }
                else if (i + 1 < text.Length && text[i + 1] == Quote)
                {
                    field.Append(Quote);
                    i++;
                }
                else
                {
                    inQuotes = false;
                }
            }
            else if (c == Quote)
            {
                inQuotes = true;
            }
            else if (c == delimiter)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if (c == CarriageReturn || c == LineFeed)
            {
                if (c == CarriageReturn && i + 1 < text.Length && text[i + 1] == LineFeed)
                {
                    i++;
                }

                row.Add(field.ToString());
                field.Clear();
                rows.Add(row);
                row = [];
            }
            else
            {
                field.Append(c);
            }
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }

    public static string Write(IEnumerable<IReadOnlyList<string>> rows, char delimiter)
    {
        var builder = new StringBuilder();
        foreach (var row in rows)
        {
            for (var i = 0; i < row.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(delimiter);
                }

                AppendField(builder, row[i], delimiter);
            }

            builder.Append(LineBreak);
        }

        return builder.ToString();
    }

    private static void AppendField(StringBuilder builder, string field, char delimiter)
    {
        if (field.IndexOfAny([delimiter, Quote, CarriageReturn, LineFeed]) < 0)
        {
            builder.Append(field);
            return;
        }

        builder.Append(Quote).Append(field.Replace("\"", "\"\"", StringComparison.Ordinal)).Append(Quote);
    }
}
