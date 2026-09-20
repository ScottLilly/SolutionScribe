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
    private static readonly (string Name, string SpdxId, string Url)[] s_licenses =
    [
        ("Apache License, Version 2.0", "Apache-2.0", "https://opensource.org/license/apache-2-0"),
        ("Common Development and Distribution License 1.0", "CDDL-1.0", "https://opensource.org/license/cddl-1-0"),
        ("Eclipse Public License version 2.0", "EPL-2.0", "https://opensource.org/license/epl-2-0"),
        ("GNU General Public License version 2", "GPL-2.0-only", "https://opensource.org/license/gpl-2-0"),
        ("GNU General Public License version 3", "GPL-3.0-only", "https://opensource.org/license/gpl-3-0"),
        ("GNU Lesser General Public License version 2.1", "LGPL-2.1-only", "https://opensource.org/license/lgpl-2-1"),
        ("GNU Lesser General Public License version 3", "LGPL-3.0-only", "https://opensource.org/license/lgpl-3-0"),
        ("GNU Library General Public License version 2", "LGPL-2.0-only", "https://opensource.org/license/lgpl-2-0"),
        ("Mozilla Public License 2.0", "MPL-2.0", "https://opensource.org/license/mpl-2-0"),
        ("The 2-Clause BSD License", "BSD-2-Clause", "https://opensource.org/license/bsd-2-clause"),
        ("The 3-Clause BSD License", "BSD-3-Clause", "https://opensource.org/license/bsd-3-clause"),
        ("The MIT License", "MIT", "https://opensource.org/license/mit")
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
