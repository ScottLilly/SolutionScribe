using System;
using System.Collections.Generic;
using System.Text;

namespace SolutionScribe.Core.Models;

/// <summary>
/// What the templates need to know about the project they are being written for. The dialog asks
/// for these once, and every template written in that pass is filled in from them.
/// </summary>
public class ProjectDetails
{
    public const string GITHUB_USER_PLACEHOLDER = "<github user>";
    public const string REPOSITORY_PLACEHOLDER = "<repository>";
    public const string NUGET_PACKAGE_PLACEHOLDER = "<nuget package>";
    public const string SECURITY_EMAIL_PLACEHOLDER = "<security email>";

    private const string IF_MARKER = "<!--#if ";
    private const string END_MARKER = "<!--#endif-->";
    private const string NUGET_CONDITION = "nuget";
    private const string APP_CONDITION = "app";

    private static readonly string[] s_placeholders =
    [
        GITHUB_USER_PLACEHOLDER,
        REPOSITORY_PLACEHOLDER,
        NUGET_PACKAGE_PLACEHOLDER,
        SECURITY_EMAIL_PLACEHOLDER
    ];

    public ProjectDetails(string gitHubUser, string repository, string nuGetPackage, string securityEmail)
    {
        GitHubUser = gitHubUser;
        Repository = repository;
        NuGetPackage = nuGetPackage;
        SecurityEmail = securityEmail;
    }

    public string GitHubUser { get; }
    public string Repository { get; }

    /// <summary>Empty for a project that publishes no package, which is what makes it an app.</summary>
    public string NuGetPackage { get; }

    public string SecurityEmail { get; }

    public bool IsNuGetPackage => !string.IsNullOrWhiteSpace(NuGetPackage);

    /// <summary>
    /// True when the template has anything for these details to fill in, so a command can skip
    /// asking for files that would not use the answers.
    /// </summary>
    public static bool HasPlaceholders(string template)
    {
        foreach (string placeholder in s_placeholders)
        {
            if (template.IndexOf(placeholder, StringComparison.Ordinal) >= 0)
            {
                return true;
            }
        }

        return template.IndexOf(IF_MARKER, StringComparison.Ordinal) >= 0;
    }

    /// <summary>
    /// The template with its conditional blocks resolved and its placeholders filled in.
    /// </summary>
    public string PopulateText(string template) =>
        ResolveConditionalBlocks(template)
            .Replace(GITHUB_USER_PLACEHOLDER, GitHubUser)
            .Replace(REPOSITORY_PLACEHOLDER, Repository)
            .Replace(NUGET_PACKAGE_PLACEHOLDER, NuGetPackage)
            .Replace(SECURITY_EMAIL_PLACEHOLDER, SecurityEmail);

    /// <summary>
    /// Keeps or drops the lines between <c>&lt;!--#if nuget--&gt;</c> or
    /// <c>&lt;!--#if app--&gt;</c> and <c>&lt;!--#endif--&gt;</c>, and removes the markers either
    /// way. A condition this does not recognize keeps its lines, because a template is the user's
    /// file and silently deleting part of it is the worse mistake. Blocks do not nest.
    /// </summary>
    private string ResolveConditionalBlocks(string template)
    {
        if (template.IndexOf(IF_MARKER, StringComparison.Ordinal) < 0)
        {
            return template;
        }

        var kept = new List<string>();
        bool? keepingBlock = null;

        foreach (string line in SplitLines(template))
        {
            string trimmed = line.Trim();

            if (trimmed.StartsWith(IF_MARKER, StringComparison.Ordinal) && trimmed.EndsWith("-->", StringComparison.Ordinal))
            {
                keepingBlock = IsConditionMet(ConditionOf(trimmed));

                continue;
            }

            if (trimmed.Equals(END_MARKER, StringComparison.Ordinal))
            {
                keepingBlock = null;

                continue;
            }

            if (keepingBlock != false)
            {
                kept.Add(line);
            }
        }

        return string.Join(Environment.NewLine, kept);
    }

    private static string ConditionOf(string markerLine) =>
        markerLine.Substring(IF_MARKER.Length, markerLine.Length - IF_MARKER.Length - "-->".Length).Trim();

    private bool IsConditionMet(string condition)
    {
        if (condition.Equals(NUGET_CONDITION, StringComparison.OrdinalIgnoreCase))
        {
            return IsNuGetPackage;
        }

        if (condition.Equals(APP_CONDITION, StringComparison.OrdinalIgnoreCase))
        {
            return !IsNuGetPackage;
        }

        return true;
    }

    /// <summary>
    /// Splits on either line ending without leaving the stray carriage returns that
    /// <c>Split('\n')</c> does, because the result is written straight to a file.
    /// </summary>
    private static IEnumerable<string> SplitLines(string text)
    {
        var line = new StringBuilder();

        for (int index = 0; index < text.Length; index++)
        {
            char character = text[index];

            if (character == '\r' || character == '\n')
            {
                if (character == '\r' && index + 1 < text.Length && text[index + 1] == '\n')
                {
                    index++;
                }

                yield return line.ToString();
                line.Clear();

                continue;
            }

            line.Append(character);
        }

        yield return line.ToString();
    }
}
