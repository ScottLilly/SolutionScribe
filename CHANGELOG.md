# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Support for Visual Studio 2026. The extension installs into both Visual Studio 2022 and Visual Studio 2026.
- Your own templates. Put a file in your template folder, `%AppData%\Solution Scribe\Templates` unless you change it under Tools > Options, and it is used instead of the built-in one. Anything you have not copied falls back to the built-in template.
- "Export templates for editing", which writes the built-in templates into that folder to start from, leaves alone anything already there, and opens the folder. It is the one command that does not need an open solution.
- A Tools > Options > Solution Scribe page, holding the default copyright holder, the GitHub user, the security contact address and the template folder.
- The templates are filled in rather than written with their placeholders intact. A dialog asks for the repository name, GitHub user, NuGet package name and security contact address, taking the repository name and user from the solution's git remote when it has one. A command writing several files asks once, and a template with nothing to fill in does not ask.
- README.md is written for an application or for a NuGet package, depending on whether a package name was given, instead of carrying commented-out alternatives for both.
- "Create all documentation files", which writes every file in one pass and leaves alone the ones that are already there.
- "Create SECURITY file", and a SECURITY.md template covering supported versions and how to report a vulnerability privately.
- "Create GitHub templates", which writes `.github\ISSUE_TEMPLATE\bug_report.md`, `.github\ISSUE_TEMPLATE\feature_request.md` and `.github\PULL_REQUEST_TEMPLATE.md`.
- Five licenses from GitHub's license picker that were missing: GNU Affero General Public License version 3, Boost Software License 1.0, Creative Commons Zero 1.0 Universal, ISC License, and The Unlicense. There are now seventeen.
- A created file opens in the editor, and the status bar names what was written.
- A Cancel button on the license dialog, with Enter and Esc bound to it and to Create File.

### Changed
- The license dialog follows the Visual Studio theme, font and DPI. Dark theme users no longer get a white dialog.
- Settings live in the Visual Studio settings store, and travel through Import and Export Settings, rather than in a JSON file under AppData. A copyright holder saved by an earlier build is not carried over.
- The getting started guide Visual Studio opens after installing is now the repository README rather than a document shipped inside the VSIX, so it cannot fall behind what the extension does. The installed extension takes 1.5 MB less on disk.
- The license dialog is modal to Visual Studio, so it can no longer end up behind the IDE.
- The copyright year and holder fields are disabled, with a note saying why, for licenses whose text has nowhere to put them. They used to accept input and then ignore it.
- The license list no longer accepts typed text that matches nothing.
- A command asks before replacing a file that already exists, and the commands are hidden when no solution is open.
- Every license text now matches its canonical version at [SPDX](https://spdx.org/licenses/).

### Fixed
- Defects in the CODE_OF_CONDUCT, CONTRIBUTING and README templates.

## [1.0.0] - 2026-09-20
### Added
- First release. Commands under Tools > Solution Scribe to create LICENSE.txt, README.md, CHANGELOG.md, CONTRIBUTING.md and CODE_OF_CONDUCT.md in the solution folder, each added to the solution's Solution Items folder.
- Twelve licenses to choose from, with the copyright year and holder filled into the ones that have a copyright line. The dialog remembers the last copyright holder used.
