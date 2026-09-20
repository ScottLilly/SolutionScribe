# SolutionScribe

@~/.claude/rules/programming.md
@~/.claude/rules/csharp.md
@~/.claude/rules/github.md

Project-specific guidance for this repo. Rules that apply to more than one project live in the
user-level `CLAUDE.md` and in the imported files above; this file is only for things particular
to SolutionScribe.

## Layout

```
SolutionScribe.sln
  SolutionScribe/            VSIX, extension package, commands, dialogs, options page
  SolutionScribe.Core/       licenses, templates, placeholders. No Visual Studio dependency
  Tests.SolutionScribe.Core/ MSTest coverage of SolutionScribe.Core
docs/                        design notes and architecture
tools/                       scripts and utilities that are not part of the build
```

`dotnet build` cannot build the VSIX project. Build the solution with `MSBuild.exe -restore` from
a Visual Studio install, and run `dotnet test Tests.SolutionScribe.Core` on its own.

All markdown except this file and `README.md` lives in `docs/`.

## Writing documents in docs/

- **Be terse.** Long documents do not get read. Cut preamble and restatement.
- **Do not speculate past what I told you.** Do not turn three sentences into three pages of
  inferred rationale or decisions I never made.
- **Mark inference as inference.** Tag it `*(inference)*` so my intent is distinguishable from
  your reading of it.
- A short list beats prose. One concrete example beats a general explanation.
- If a document has grown unwieldy, say so and offer to consolidate rather than adding to it.
- No em dashes, no en dashes, no smart quotes.

## Settled work leaves no trace

When something is **built**, the documents describe it as it is now and nothing records that it was
ever proposed. No superseded sections, no struck-through questions, no "amended on such a date"
banners. Git holds the history, and the issue that asked for it holds the rest.

When something is **decided against**, leave one brief line under `## Decided against` in
`docs/ARCHITECTURE.md` saying what it was and why not, with the reasoning, so it does not get
re-proposed. Where the decision concerns one member, it goes on that member's doc comment instead,
because that is what somebody reads before changing it. A GitHub issue that is dropped is closed as
not planned and taken off its milestone, so it does not count as that milestone's work.

Unbuilt work lives in GitHub Issues. This repository had a `docs/BACKLOG.md` and retired it in
September 2026; do not reintroduce one.
