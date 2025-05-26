# Contributing to [Your Repository Name]

Thank you for contributing to [Your Repository Name], an open-source C# project! This guide outlines how to contribute effectively, ensuring high-quality code and a collaborative environment. Please read this before submitting contributions.

## Code of Conduct

We foster an inclusive and respectful community. All contributors must follow our [Code of Conduct](CODE_OF_CONDUCT.md). Please review it to understand expected behavior.

## How to Contribute

### 1. Setting Up the Project

To contribute, set up the project locally:

- **Fork the Repository**: Click “Fork” on the repository’s GitHub page to create a copy in your account.
- **Clone Your Fork**:
  ```bash
  git clone https://github.com/your-username/[your-repo-name].git
  cd [your-repo-name]
  ```
- **Open the Solution**: Open the `.sln` file in Visual Studio (2022 or later recommended).
- **Install Dependencies**: Ensure you have the .NET SDK (version [specify, e.g., 8.0 or latest]). All dependencies are Microsoft-provided NuGet packages.
- **Build and Verify**: Build the solution (`Ctrl+Shift+B` in Visual Studio or `dotnet build`) to ensure the setup works.

### 2. Finding Issues to Work On

We use GitHub Issues to track bugs, features, and tasks:

- **Browse Issues**: Visit the [Issues tab](https://github.com/[your-username]/[your-repo-name]/issues) and look for “good first issue” or “help wanted” labels.
- **Create an Issue**: Propose a bug fix or feature by creating an issue using our templates (see [Issue Templates](#issue-templates)).
- **Wait for Approval**: Only issues in the “Ready for Work” column of our [GitHub Project](https://github.com/[your-username]/[your-repo-name]/projects) are approved for contributions. The repository owner or approved maintainers move issues to this status.

### 3. Creating a Pull Request (PR)

All code contributions must be submitted via pull requests:

- **Create a Separate Branch**:
  - Work in a new branch, not `master`. Name it descriptively, e.g., `fix/issue-123` or `feature/add-auth`.
  - Example:
    ```bash
    git checkout -b fix/issue-123
    ```
- **Pull the Latest Code**:
  - Before pushing, ensure your branch has the latest `master` branch code:
    ```bash
    git remote add upstream https://github.com/[original-owner]/[your-repo-name].git
    git fetch upstream
    git rebase upstream/master
    ```
  - Resolve conflicts and verify functionality.
- **Link to an Approved Issue**:
  - Your PR must address an issue in “Ready for Work” status. Reference it in the PR description, e.g., `Closes #123`.
- **Make Changes**:
  - Follow our [Coding Standards](https://github.com/ScottLilly/CodingStandards/blob/master/CSharp/CSharpStandards.md).
  - Use only Microsoft-provided NuGet packages (e.g., avoid third-party libraries unless approved).
  - Add unit tests for new or changed code using MSTest or xUnit. If the project lacks tests, discuss with maintainers in the issue.
- **Test Your Changes**:
  - Run `dotnet test` to ensure tests pass (if tests exist).
  - Verify the solution builds (`dotnet build`) and runs correctly.
- **Push and Create a PR**:
  - Push your branch:
    ```bash
    git push origin fix/issue-123
    ```
  - Open a PR on GitHub, targeting the repository’s `master` branch.
  - Complete the PR template (see [Pull Request Template](#pull-request-template)).
- **PR Requirements**:
  - Must be linked to an approved issue.
  - Must pass CI checks (e.g., build, tests, linting).
  - Requires at least one maintainer approval.
  - Address review feedback promptly.

### 4. Issue Templates

To create an issue:

- Go to the [Issues tab](https://github.com/[your-username]/[your-repo-name]/issues) and click “New Issue.”
- Choose a template (e.g., “Bug Report” or “Feature Request”) and fill out all fields.

Maintainers review issues and move approved ones to “Ready for Work.”

### 5. Pull Request Template

PRs use a template to describe changes. Include:

- Summary of changes.
- Linked issue (e.g., `Closes #123`).
- Testing details or screenshots (if applicable).

### 6. Development Guidelines

- **Coding Standards**: Follow our [CODING_STANDARDS.md](CODING_STANDARDS.md).
- **Dependencies**: Use only Microsoft NuGet packages.
- **Testing**: Add unit tests for changes using MSTest, NUnit, or xUnit. If tests don’t exist, note this in the issue or PR.
- **Documentation**: Update README or code comments for new features or changes.
- **Commit Messages**: Use clear messages, e.g., `Fix: Resolve null reference in auth module`.

### 7. Communication

All communication happens on GitHub:

- Use the [Issues tab](https://github.com/[your-username]/[your-repo-name]/issues) for questions or discussions.
- Comment on issues or PRs for clarifications.
- Check [GitHub Discussions](https://github.com/[your-username]/[your-repo-name]/discussions) for broader topics (if enabled).

### 8. Legal

Contributions are licensed under the [MIT License](LICENSE). By contributing, you agree to this license.

## Thank You!

Your contributions make [Your Repository Name] better. Thank you for your time and effort!