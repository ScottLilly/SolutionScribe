namespace SolutionScribe.Core.Models;

/// <summary>
/// The owner and repository halves of a GitHub remote URL, as in
/// <c>https://github.com/ScottLilly/SolutionScribe.git</c>.
/// </summary>
public class GitHubRepository
{
    public GitHubRepository(string owner, string name)
    {
        Owner = owner;
        Name = name;
    }

    public string Owner { get; }
    public string Name { get; }
}
