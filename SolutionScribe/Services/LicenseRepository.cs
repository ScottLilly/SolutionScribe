using SolutionScribe.Models;
using System.Collections.Generic;

namespace SolutionScribe.Services;

internal static class LicenseRepository
{
    internal static List<LicenseDetails> GetLicenseDetailsList()
    {
        List<LicenseDetails> licenseDetailsList = [];

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "Apache License, Version 2.0",
            SPDXID = "Apache-2.0",
            LicenseUrl = "https://opensource.org/license/apache-2-0",
            LicenseText = @"
Copyright <year> <copyright holder>

Licensed under the Apache License, Version 2.0 (the ""License"");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License."
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "Common Development and Distribution License 1.0",
            SPDXID = "CDDL-1.0",
            LicenseUrl = "https://opensource.org/license/cddl-1-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "Eclipse Public License version 2.0",
            SPDXID = "EPL-2.0",
            LicenseUrl = "https://opensource.org/license/epl-2-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "GNU General Public License version 2",
            SPDXID = "GPL-2.0",
            LicenseUrl = "https://opensource.org/license/gpl-2-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "GNU General Public License version 3",
            SPDXID = "GPL-3.0-only",
            LicenseUrl = "https://opensource.org/license/lgpl-3-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "GNU Lesser General Public License version 2.1",
            SPDXID = "LGPL-2.1",
            LicenseUrl = "https://opensource.org/license/lgpl-2-1",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "GNU Lesser General Public License version 3",
            SPDXID = "LGPL-3.0-only",
            LicenseUrl = "https://opensource.org/license/lgpl-3-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "GNU Library General Public License version 2",
            SPDXID = "LGPL-2.0-only",
            LicenseUrl = "https://opensource.org/license/lgpl-2-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "Mozilla Public License 2.0",
            SPDXID = "MPL-2.0",
            LicenseUrl = "https://opensource.org/license/mpl-2-0",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "The 2-Clause BSD License",
            SPDXID = "BSD-2-clause",
            LicenseUrl = "https://opensource.org/license/bsd-2-clause",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "The 3-Clause BSD License",
            SPDXID = "BSD-3-clause",
            LicenseUrl = "https://opensource.org/license/bsd-3-clause",
            LicenseText = @""
        });

        licenseDetailsList.Add(new LicenseDetails
        {
            LicenseName = "The MIT License",
            SPDXID = "MIT",
            LicenseUrl = "https://opensource.org/license/mit",
            LicenseText = @""
        });

        return licenseDetailsList;
    }
}
