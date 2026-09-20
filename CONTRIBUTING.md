# Contributing to SolutionScribe

Thank you for contributing to SolutionScribe, an open-source project! This guide outlines how to contribute effectively, ensuring high-quality code and a collaborative environment. Please read this before submitting contributions.

## Code of Conduct

Be respectful and constructive in issues and pull requests. This repository has no CODE_OF_CONDUCT.md, which is a deliberate choice rather than an oversight.

## How to Contribute

### 1. Setting Up the Project

To contribute, set up the project locally:

- **Fork the Repository**: Click "Fork" on the repository's GitHub page to create a copy in your account.
- **Clone Your Fork**:
  ```bash
  git clone https://github.com/[your-user-name]/SolutionScribe.git
  cd SolutionScribe
  ```
- **Open the Solution**: Open `SolutionScribe.sln` in Visual Studio 2022 or Visual Studio 2026, with the **Visual Studio extension development** workload installed. The VSIX project needs it. The test project targets net48 so that the solution still opens in 2022, which ships a .NET 9 SDK.
- **Build and Verify**: Build the solution (`Ctrl+Shift+B`). `dotnet build` cannot build this solution, because the VSIX project uses MSBuild tasks that only exist on .NET Framework MSBuild. From a command line, use `MSBuild.exe SolutionScribe.sln -restore` from a Visual Studio install.
- **Run It**: Press `F5` to launch an experimental instance of Visual Studio with the extension loaded. Open any solution there and the commands appear under Tools > Solution Scribe.

### 2. Finding Issues to Work On

We use GitHub Issues to track bugs, features, and tasks:

- **Browse Issues**: Visit the [Issues tab](https://github.com/ScottLilly/SolutionScribe/issues).
- **Create an Issue**: Propose a bug fix or feature by creating an issue using our templates (see [Issue Templates](#4-issue-templates)).
- **Wait for Approval**: Work only on issues a maintainer has approved for contribution. Comment on the issue and ask before starting. Approval is given as a comment on the issue itself.

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
    git remote add upstream https://github.com/ScottLilly/SolutionScribe.git
    git fetch upstream
    git rebase upstream/master
    ```
  - Resolve conflicts and verify functionality.
- **Link to an Approved Issue**:
  - Your PR must address an issue a maintainer has approved. Reference it in the PR description, e.g., `Closes #123`.
- **Make Changes**:
  - Match the style of the surrounding code. [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) describes the project layout and the one rule that is not negotiable: `SolutionScribe.Core` must not reference `Microsoft.VisualStudio.*`, `EnvDTE` or `Community.VisualStudio.Toolkit`.
  - Add MSTest coverage in `Tests.SolutionScribe.Core` for anything you put in `SolutionScribe.Core`. Code in the VSIX project cannot be unit tested and is not expected to be.
- **Test Your Changes**:
  - Run `dotnet test Tests.SolutionScribe.Core` to ensure tests pass. That project builds on its own, without the VSSDK.
  - Verify the whole solution still builds, and that the change behaves as intended in the experimental instance.
- **Push and Create a PR**:
  - Push your branch:
    ```bash
    git push origin fix/issue-123
    ```
  - Open a PR on GitHub, targeting this repository's `master` branch.
  - Complete the PR template (see [Pull Request Template](#5-pull-request-template)).
- **PR Requirements**:
  - Must be linked to an approved issue.
  - Must build, and `dotnet test Tests.SolutionScribe.Core` must pass.
  - Requires maintainer approval.
  - Address review feedback promptly.

### 4. Issue Templates

To create an issue:

- Go to the [Issues tab](https://github.com/ScottLilly/SolutionScribe/issues) and click "New Issue."
- Choose a template ("Bug Report" or "Feature Request") and fill out all fields.

Maintainers review issues and say on the issue when one is approved for contribution.

### 5. Pull Request Template

PRs use a template to describe changes. Include:

- Summary of changes.
- Linked issue (e.g., `Closes #123`).
- Testing details or screenshots (if applicable).

### 6. Development Guidelines

- **Adding a license**: put the canonical text from [SPDX](https://spdx.org/licenses/) in `SolutionScribe.Core/Licenses/<spdx-id>.txt`, rename the copyright holder token to `<copyright holder>`, and add a row to `LicenseRepository`. The embedded resource is picked up automatically, and the tests check that the list and the files agree.
- **Adding a template**: put it in `SolutionScribe.Core/Templates/`, add a method to `TemplateFileRepository`, and add a command deriving from `CreateSolutionFileCommandBase`.
- **Dependencies**: think hard before adding one. An extension shares a process with Visual Studio, so a package version that disagrees with the one VS loads is a runtime failure rather than a build error, on a machine you cannot debug. `SolutionScribe.Core` has no package references at all, and everything it has needed so far has been a few lines of its own code instead.
- **Generated files**: `source.extension.cs` and `VSCommandTable.cs` are written by VSIX Synchronizer from the manifest and the `.vsct`. Edit the source files, not the generated ones, and open the solution in Visual Studio so the tool regenerates them.
- **Documentation**: update the README or `docs/` when a change affects what the extension does.
- **Commit Messages**: use clear messages, e.g., `Fix: Resolve null reference in auth module`.

### 7. Communication

All communication happens on GitHub:

- Use the [Issues tab](https://github.com/ScottLilly/SolutionScribe/issues) for questions or discussions.
- Comment on issues or PRs for clarifications.

### 8. Legal

Contributions are licensed under the terms in the [LICENSE.txt](LICENSE.txt) file. By contributing, you agree to those terms.

## Thank You!

Your contributions make SolutionScribe better. Thank you for your time and effort!
