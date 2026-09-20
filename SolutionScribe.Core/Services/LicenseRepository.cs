using SolutionScribe.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolutionScribe.Core.Services;

public static class LicenseRepository
{
    private const string RESOURCE_PREFIX = "SolutionScribe.Core.Licenses.";

    // The SPDX id is also the name of the embedded text file, so each text can be
    // diffed against the canonical version at https://spdx.org/licenses/ .
    //
    // The name is the license's title on its Open Source Initiative page, and the URL is that
    // page. CC0-1.0 is the exception: the OSI rejected it, so it has no page there and points at
    // the Creative Commons deed instead.
    //
    // The order is the order the dialog offers them in.
    private static readonly (string Name, string SpdxId, string Url)[] s_licenses =
    [
        ("Apache License, Version 2.0", "Apache-2.0", "https://opensource.org/license/apache-2-0"),
        ("Boost Software License 1.0", "BSL-1.0", "https://opensource.org/license/bsl-1-0"),
        ("Common Development and Distribution License 1.0", "CDDL-1.0", "https://opensource.org/license/cddl-1-0"),
        ("Creative Commons Zero 1.0 Universal", "CC0-1.0", "https://creativecommons.org/publicdomain/zero/1.0/"),
        ("Eclipse Public License version 2.0", "EPL-2.0", "https://opensource.org/license/epl-2-0"),
        ("GNU Affero General Public License version 3", "AGPL-3.0-only", "https://opensource.org/license/agpl-v3"),
        ("GNU General Public License version 2", "GPL-2.0-only", "https://opensource.org/license/gpl-2-0"),
        ("GNU General Public License version 3", "GPL-3.0-only", "https://opensource.org/license/gpl-3-0"),
        ("GNU Lesser General Public License version 2.1", "LGPL-2.1-only", "https://opensource.org/license/lgpl-2-1"),
        ("GNU Lesser General Public License version 3", "LGPL-3.0-only", "https://opensource.org/license/lgpl-3-0"),
        ("GNU Library General Public License version 2", "LGPL-2.0-only", "https://opensource.org/license/lgpl-2-0"),
        ("ISC License", "ISC", "https://opensource.org/license/isc-license-txt"),
        ("Mozilla Public License 2.0", "MPL-2.0", "https://opensource.org/license/mpl-2-0"),
        ("The 2-Clause BSD License", "BSD-2-Clause", "https://opensource.org/license/bsd-2-clause"),
        ("The 3-Clause BSD License", "BSD-3-Clause", "https://opensource.org/license/bsd-3-clause"),
        ("The MIT License", "MIT", "https://opensource.org/license/mit"),
        ("The Unlicense", "Unlicense", "https://opensource.org/license/unlicense")
    ];

    public static List<LicenseDetails> GetLicenseDetailsList() =>
        s_licenses
            .Select(license => new LicenseDetails(
                license.Name,
                license.SpdxId,
                license.Url,
                LoadEmbeddedText(license.SpdxId)))
            .ToList();

    private static string LoadEmbeddedText(string spdxId)
    {
        var assembly = typeof(LicenseRepository).Assembly;

        using var stream = assembly.GetManifestResourceStream($"{RESOURCE_PREFIX}{spdxId}.txt");

        if (stream == null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
