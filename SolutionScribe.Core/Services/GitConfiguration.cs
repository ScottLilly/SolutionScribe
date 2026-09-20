using SolutionScribe.Core.Models;
using System;
using System.IO;

namespace SolutionScribe.Core.Services;

/// <summary>
/// Reads the GitHub owner and repository out of a working copy's <c>.git\config</c>, so the
/// project details dialog can fill itself in rather than asking for what the repository already
/// knows.
/// </summary>
public static class GitConfiguration
{
    private const string ORIGIN_SECTION = "[remote \"origin\"]";

    /// <summary>
    /// The origin remote of the working copy holding <paramref name="directory"/>, or null when
    /// there is no working copy, no origin, or an origin that is not on GitHub. Walks up from the
    /// directory, because a solution often sits below the repository root.
    /// </summary>
    public static GitHubRepository? FindGitHubRepository(string directory)
    {
        for (var current = SafeDirectory(directory); current != null; current = current.Parent)
        {
            string configPath = Path.Combine(current.FullName, ".git", "config");

            if (!File.Exists(configPath))
            {
                continue;
            }

            try
            {
                return ParseGitHubRepository(File.ReadAllText(configPath));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // An unreadable config is the same as not having one: the dialog asks instead.
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// The GitHub owner and repository named by the origin remote in the text of a
    /// <c>.git\config</c>, or null when it names none.
    /// </summary>
    public static GitHubRepository? ParseGitHubRepository(string gitConfigText)
    {
        string? url = FindOriginUrl(gitConfigText);

        return url == null ? null : ParseGitHubUrl(url);
    }

    private static string? FindOriginUrl(string gitConfigText)
    {
        bool inOrigin = false;

        foreach (string rawLine in gitConfigText.Split('\n'))
        {
            string line = rawLine.Trim();

            if (line.StartsWith("[", StringComparison.Ordinal))
            {
                inOrigin = line.Equals(ORIGIN_SECTION, StringComparison.OrdinalIgnoreCase);

                continue;
            }

            if (!inOrigin || !line.StartsWith("url", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int separator = line.IndexOf('=');

            if (separator >= 0)
            {
                return line.Substring(separator + 1).Trim();
            }
        }

        return null;
    }

    /// <summary>
    /// Handles the four forms git writes: <c>https://github.com/owner/name(.git)</c>,
    /// <c>git@github.com:owner/name.git</c>, and the <c>ssh://</c> spelling of the last.
    /// </summary>
    private static GitHubRepository? ParseGitHubUrl(string url)
    {
        const string GITHUB_HOST = "github.com";

        int host = url.IndexOf(GITHUB_HOST, StringComparison.OrdinalIgnoreCase);

        if (host < 0)
        {
            return null;
        }

        string path = url.Substring(host + GITHUB_HOST.Length).TrimStart('/', ':');

        if (path.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
        {
            path = path.Substring(0, path.Length - ".git".Length);
        }

        string[] parts = path.TrimEnd('/').Split('/');

        return parts.Length == 2 && parts[0].Length > 0 && parts[1].Length > 0
            ? new GitHubRepository(parts[0], parts[1])
            : null;
    }

    private static DirectoryInfo? SafeDirectory(string directory)
    {
        try
        {
            return new DirectoryInfo(directory);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is PathTooLongException)
        {
            return null;
        }
    }
}
