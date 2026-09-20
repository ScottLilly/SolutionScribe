# SolutionScribe

<img align="left" width="125" height="125" style="color:white" src="https://raw.githubusercontent.com/ScottLilly/SolutionScribe/master/SolutionScribe/Resources/Icon.png">

Visual Studio extension to create documentation files for your solutions.

Solution Scribe can create these files for you: CHANGELOG.md, CODE_OF_CONDUCT.md, CONTRIBUTING.md, LICENSE.txt, README.md, SECURITY.md

It can also create GitHub's issue and pull request templates: .github\ISSUE_TEMPLATE\bug_report.md, .github\ISSUE_TEMPLATE\feature_request.md, and .github\PULL_REQUEST_TEMPLATE.md

The available licenses are:
-	Apache License, Version 2.0
-	Boost Software License 1.0
-	Common Development and Distribution License 1.0
-	Creative Commons Zero 1.0 Universal
-	Eclipse Public License version 2.0
-	GNU Affero General Public License version 3
-	GNU General Public License version 2
-	GNU General Public License version 3
-	GNU Lesser General Public License version 2.1
-	GNU Lesser General Public License version 3
-	GNU Library General Public License version 2
-	ISC License
-	Mozilla Public License 2.0
-	The 2-Clause BSD License
-	The 3-Clause BSD License
-	The MIT License
-	The Unlicense

## Project Overview

A new repository needs the same handful of files every time, and writing them by hand means
finding a license text, getting the copyright line right, and remembering what belongs in a
CONTRIBUTING file. Solution Scribe writes them from templates, into the solution you already have
open, without leaving Visual Studio.

The commands live under **Tools > Solution Scribe**. They appear only when a solution is open,
because every one of them writes into the solution folder.

## Installation

Install the VSIX file. Download the latest one from the
[Releases page](https://github.com/ScottLilly/SolutionScribe/releases), close Visual Studio, and
double click it.

A Visual Studio Marketplace listing is not published yet. A link will go here when it is.

## How to use

Open a solution, then pick a command from **Tools > Solution Scribe**:

| Command | Writes |
|---|---|
| Create all documentation files | All six files below, in one pass |
| Create LICENSE file | `LICENSE.txt`, from the license you pick |
| Create README file | `README.md` |
| Create CONTRIBUTING file | `CONTRIBUTING.md` |
| Create CHANGELOG file | `CHANGELOG.md` |
| Create CODE_OF_CONDUCT file | `CODE_OF_CONDUCT.md` |
| Create SECURITY file | `SECURITY.md` |
| Create GitHub templates | The three files under `.github\` |

Every file is written into the solution folder, next to the `.sln`, and added to the solution's
**Solution Items** folder so it is visible in Solution Explorer. That folder is created if it is
not already there. The GitHub templates go into a `.github` solution folder instead, matching
where they sit on disk. The new file then opens in the editor, because every template needs
editing before it is any use.

**Create LICENSE file** asks which license you want, along with the copyright year and holder. It
remembers the copyright holder you used last time and fills it in for you, and you can set it
yourself under Tools > Options > Solution Scribe. Licenses whose text has nowhere to put a
copyright line, such as the GNU and Mozilla ones, disable those two fields and say so.

A command that writes one file asks before replacing a file that is already there. The two
commands that write several leave existing files alone and say which ones they skipped.

## Requirements

- Visual Studio 2022 (17.x) or Visual Studio 2026 (18.x)
- 64 bit Windows

Arm64 Visual Studio is not supported. The extension declares an x64 payload only.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for how to set the project up,
which issues are open for contribution, and what a pull request needs.

## License
This project is licensed under the MIT License. See the [LICENSE file](https://github.com/ScottLilly/SolutionScribe/blob/master/LICENSE.txt) for details.

## Contact
For questions or feedback, please [open an issue here on GitHub](https://github.com/ScottLilly/SolutionScribe/issues).
