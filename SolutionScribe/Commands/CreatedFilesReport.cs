using System.Collections.Generic;
using System.Text;

namespace SolutionScribe.Commands;

/// <summary>
/// What a command that writes several files did, as one line for the status bar. A command that
/// writes a single file says so itself; this is for the ones where the interesting part is which
/// files were left alone.
/// </summary>
internal sealed class CreatedFilesReport
{
    private readonly List<string> _written = new List<string>();
    private readonly List<string> _skipped = new List<string>();

    public void Wrote(string fileName) => _written.Add(fileName);

    public void Skipped(string fileName) => _skipped.Add(fileName);

    public override string ToString()
    {
        var message = new StringBuilder("Solution Scribe ");

        message.Append(_written.Count == 0
            ? "created nothing"
            : $"created {string.Join(", ", _written)}");

        if (_skipped.Count > 0)
        {
            message.Append($". Skipped {string.Join(", ", _skipped)}, which already existed");
        }

        message.Append(".");

        return message.ToString();
    }
}
