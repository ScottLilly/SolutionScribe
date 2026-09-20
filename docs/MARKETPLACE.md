# Marketplace listing

Copy for the Visual Studio Marketplace listing, kept here because the listing is edited in a web
form that nothing in the repository can see. Update it when the extension gains a command.

Nothing here is published yet. See [What is still needed](#what-is-still-needed).

## Fields that come from the manifest

`source.extension.vsixmanifest` supplies these, so change them there and not on the listing:

| Field | Value |
|---|---|
| Name | Solution Scribe |
| Description | One sentence, in `<Description>` |
| More info | The GitHub repository |
| Release notes | `CHANGELOG.md` on GitHub |
| Tags | documentation, readme, license, changelog, contributing, code of conduct, security, github, solution, scaffolding |
| Icon | `Resources\Icon.png`, 700 x 700 |

## Naming

**No `ScottLilly.` prefix on the extension name.** The Marketplace's unique identifier is always
`Publisher.ExtensionName`, so registering the publisher as `ScottLilly` already gives
`ScottLilly.SolutionScribe`. The prefix on the NuGet packages exists because NuGet ids are one
flat namespace with nothing supplying an owner; the Marketplace has no such problem, and
prefixing on top of the publisher would both repeat it in the identifier and push the words that
matter out of the display name.

`<Identity Id>` is a separate machine level id and is already unique. It needs no prefix either.

Unresolved, and worth watching for in the publish form: what the Marketplace derives
`ExtensionName` from. If it comes from `<Identity Id>`, the permalink could end up carrying that
id's GUID.

## Overview

The text below goes in the listing's Overview, which takes Markdown.

---

Every new repository needs the same handful of files, and writing them by hand means finding a
license text, getting the copyright line right, and remembering what belongs in a CONTRIBUTING
file. Solution Scribe writes them from templates, into the solution you already have open.

**Tools > Solution Scribe**, with a solution open:

- **Create all documentation files**, or any one of them on its own.
- **README**, **LICENSE**, **CHANGELOG**, **CONTRIBUTING**, **CODE_OF_CONDUCT** and **SECURITY**.
- **GitHub templates**: a bug report, a feature request, and a pull request template, under
  `.github\`.

Each file is written next to your `.sln`, added to the solution's Solution Items folder so it is
visible in Solution Explorer, and opened in the editor ready to edit.

The LICENSE command offers seventeen licenses, including everything in GitHub's license picker,
with each text taken from its canonical version at SPDX. It fills in the copyright year and holder
for you, remembers the holder you used last time, and tells you when a license has nowhere to put
them.

Nothing is overwritten without asking.

Free, open source, and the same set of templates the repository uses for itself.

---

## What is still needed

- [ ] A real `PreviewImage`. The manifest currently points at the icon, which the Marketplace shows
      large on the listing page. It wants a screenshot of the menu or the license dialog, at least
      175 x 175.
- [ ] Screenshots for the listing and for the repository README: the Tools menu, and the license
      dialog.
- [ ] Register the publisher as `ScottLilly`, then create the listing and upload the `.vsix`.
- [ ] Check what the publish form makes the extension name, and set it to `SolutionScribe` if it
      can be set.
- [ ] Decide the Q and A setting. The choices are the Marketplace's own Q and A, GitHub Issues on
      this repository, or off. This repository already directs questions to its Issues tab.
- [ ] Add the Marketplace link to `README.md`, which says the listing is not published yet.
