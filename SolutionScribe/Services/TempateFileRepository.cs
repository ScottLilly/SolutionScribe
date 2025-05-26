using System.IO;

namespace SolutionScribe.Services;

internal static class TempateFileRepository
{
    public static string GetChangelogTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Templates.CHANGELOG.md");
    }

    public static string GetCodeOfConductTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Templates.CODE_OF_CONDUCT.md");
    }

    public static string GetContributingTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Templates.CONTRIBUTING.md");
    }

    public static string GetReadmeTemplate()
    {
        return LoadEmbeddedDefault("SolutionScribe.Templates.README.md");
    }

    private static string LoadEmbeddedDefault(string resourceName)
    {
        var assembly = typeof(SettingsRepository).Assembly;

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
