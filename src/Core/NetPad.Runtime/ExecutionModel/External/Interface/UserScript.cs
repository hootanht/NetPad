using System.IO;
using System.Reflection;

namespace NetPad.ExecutionModel.External.Interface;

/// <summary>
/// Info about current script.
/// </summary>
/// <param name="Name">The script name.</param>
/// <param name="FilePath">The full path of the script file. Empty if not a saved script.</param>
public record UserScript(string Name, string? FilePath)
{
    public string RunLocation => Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;
}
