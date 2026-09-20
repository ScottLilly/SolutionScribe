using System.Runtime.InteropServices;

namespace SolutionScribe.Options;

/// <summary>
/// Holds the page types named by the <c>ProvideOptionPage</c> attributes on
/// <see cref="SolutionScribePackage"/>. A page has to be COM visible for Visual Studio to create
/// it, which is the only reason these exist separately from the option models they show.
/// </summary>
internal partial class OptionsProvider
{
    [ComVisible(true)]
    public class GeneralOptionsPage : BaseOptionPage<GeneralOptions>
    {
    }
}
