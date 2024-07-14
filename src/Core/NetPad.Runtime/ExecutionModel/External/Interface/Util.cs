using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NetPad.Presentation;
using NetPad.Presentation.Html;
using NetPad.Utilities;
using O2Html.Dom;

namespace NetPad.ExecutionModel.External.Interface;

public static class Util
{
    static Util()
    {
        ScriptStopwatch = new Stopwatch();
    }

    public static UserScript CurrentScript { get; private set; } = null!;
    public static Stopwatch ScriptStopwatch { get; }
    public static TimeSpan ElapsedTime => ScriptStopwatch.Elapsed;

    public static int ParentProcessID { get; set; }
    //public static string CurrentCxString =>
    //public static string CurrentDataContext =>
    //public static System.Transactions.IsolationLevel? TransactionIsolationLevel { get; set; }
    //public static void ClearResults()
    //public static ExpandoObject ToExpando(object value, IEnumerable<MemberInfo> include)
    //public static Try

    public static Version CurrentRuntime => Environment.Version;
    public static OperatingSystem OsVersion => Environment.OSVersion;
    public static string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
    public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;
    public static string OSDescription => RuntimeInformation.OSDescription;
    public static Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
    public static Architecture OSArchitecture => RuntimeInformation.OSArchitecture;

    public static void Init(string currentScriptName, string currentScriptFilePath)
    {
        CurrentScript = new UserScript(
            currentScriptName,
            string.IsNullOrWhiteSpace(currentScriptFilePath) ? null : currentScriptFilePath
        );
    }

    public static void OpenBrowser(string url) => ProcessUtil.OpenWithDefaultApp(url);
    public static void OpenFolder(string fullPath) => ProcessUtil.OpenWithDefaultApp(fullPath);
    public static void OpenFile(string fullPath) => ProcessUtil.OpenWithDefaultApp(fullPath);
    public static void OpenWithDefaultApp(string path) => ProcessUtil.OpenWithDefaultApp(path);

    public static string ToHtmlString(object objectToSerialize)
    {
        return HtmlPresenter.Serialize(objectToSerialize);
    }

    public static Element ToHtmlElement(object objectToSerialize)
    {
        return HtmlPresenter.SerializeToElement(objectToSerialize);
    }



    /// <summary>
    /// Dumps this object to the results console.
    /// </summary>
    /// <param name="o">The object to dump.</param>
    /// <param name="title">If specified, will add a title heading to the result.</param>
    /// <param name="css">If specified, will add the specified CSS classes to the result.</param>
    /// <param name="code">If specified, assumes the dump target is a code string of this language and will
    /// render with syntax highlighting. See https://github.com/highlightjs/highlight.js/blob/main/SUPPORTED_LANGUAGES.md for supported languages.</param>
    /// <param name="clear">If specified, will remove the result after specified milliseconds.</param>
    /// <returns>The same object being dumped.</returns>
    [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("o")]
    public static T? Dump<T>(T? o, string? title = null, string? css = null, string? code = null, int? clear = null)
    {
        return DumpExtension.Dump(o, title, css, code, clear);
    }

    /// <summary>
    /// Dumps this <see cref="Span{T}"/> to the results view.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to dump.</param>
    /// <param name="title">An optional title for the result.</param>
    /// <param name="css">If specified, will be added as CSS classes to the result.</param>
    /// <param name="clear">If specified, will remove the result after specified milliseconds.</param>
    /// <returns>The <see cref="Span{T}"/> being dumped.</returns>
    public static Span<T> Dump<T>(Span<T> span, string? title = null, string? css = null, int? clear = null)
    {
        return DumpExtension.Dump(span, title, css, clear);
    }

    /// <summary>
    /// Dumps this <see cref="ReadOnlySpan{T}"/> to the results view.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to dump.</param>
    /// <param name="title">An optional title for the result.</param>
    /// <param name="css">If specified, will be added as CSS classes to the result.</param>
    /// <param name="clear">If specified, will remove the result after specified milliseconds.</param>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> being dumped.</returns>
    public static ReadOnlySpan<T> Dump<T>(ReadOnlySpan<T> span, string? title = null, string? css = null, int? clear = null)
    {
        return DumpExtension.Dump(span, title, css, clear);
    }
}
