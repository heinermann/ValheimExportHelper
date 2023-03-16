using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  class FixCodeFiles : PostExporterEx
  {
    private static readonly string[] ScriptDirs = new[] { "MonoScript", "Scripts" };

    public override void Export()
    {
      LogInfo("Fixing MonoScript/.cs source files (if any)");

      foreach (var dir in ScriptDirs)
      {
        string scriptDir = Path.Join(CurrentRipper.Settings.AssetsPath, dir);
      
        DeleteStandardLibraries(scriptDir);
        WholeCodebaseFixes(scriptDir);
        OneOffCodeFixes(scriptDir);
      }
    }

    private readonly string[] LibsToDelete =
    {
      "Microsoft.CSharp", "Mono.Posix", "XGamingRuntime"
    };
    private void DeleteStandardLibraries(string scriptsDir)
    {
      LogInfo("Deleting standard libraries");

      foreach (var lib in LibsToDelete)
      {
        TryDelete(Path.Join(scriptsDir, lib));
        TryDelete(Path.Join(scriptsDir, $"{lib}.dll"));
      }
    }

    private string FixStructLayout(string file)
    {
      file = file.Replace("StructLayout(0", "StructLayout(LayoutKind.Sequential");
      file = file.Replace("StructLayout(2", "StructLayout(LayoutKind.Explicit");
      return file;
    }

    /**
     * 1. ^(\s.*\bevent [\w<>., ]+? \w+)\r?\n                    Event definition
     * 2. (\s+)\{\r?\n                                        First open brace + spacing capture
     * 3. (\2\s+)\[CompilerGenerated\]\r?\n                   First CompilerGenerated tag + spacing capture
     * 4. \3add\r?\n                                          "add" declaration
     * 5. \3\{[\w\W]+?\r?\n\3\}\r?\n                          "add" block capture
     * 6. \3\[CompilerGenerated\]\r?\n                        Second CompilerGenerated tag
     * 7. \3remove\r?\n                                       "remove" declaration
     * 8. \3\{[\w\W]+?\r?\n\3\}\r?\n                          "remove" block capture
     * 9. \2}                                                 Closing first brace.
     */
    const string EventRegex = @"^(\s.*\bevent [\w<>., ]+? \w+)\r?\n(\s+)\{\r?\n(\2\s+)\[CompilerGenerated\]\r?\n\3add\r?\n\3\{[\w\W]+?\r?\n\3\}\r?\n\3\[CompilerGenerated\]\r?\n\3remove\r?\n\3\{[\w\W]+?\r?\n\3\}\r?\n\2\}";
    private string FixEvents(string file)
    {
      return Regex.Replace(file, EventRegex, @"$1;", RegexOptions.Multiline);
    }

    const string UncheckedRegex = @"(public override int GetHashCode\(\)\r?\n\s+\{\r?\n\s+return) \(";
    private string FixUncheckedHashCode(string file)
    {
      return Regex.Replace(file, UncheckedRegex, @"$1 unchecked (", RegexOptions.Multiline);
    }

    const string AmbiguousDebugRegex = @"([^.\w])(Debug.Log(Warning|Error)?\()";
    private string FixAmbiguousDebugCalls(string file)
    {
      return Regex.Replace(file, AmbiguousDebugRegex, @"$1UnityEngine.$2", RegexOptions.Multiline);
    }

    const string SpecialNameFnRegex = @"^(\s*)\[SpecialName\]\r?\n\1Transform .*?\r?\n\1\{[\w\W]+?\r?\n\1\}\r?\n";
    private string FixSpecialNameFn(string file)
    {
      return Regex.Replace(file, SpecialNameFnRegex, "", RegexOptions.Multiline);
    }

    const string BadCtorRegex = @"^\s+base\._[0-9A-F]{4}ctor\(.*$";
    private string FixBadCtor(string file)
    {
      return Regex.Replace(file, BadCtorRegex, "", RegexOptions.Multiline);
    }

    const string ReadOnlyAttrRegex = @"\[IsReadOnly\]";
    private string FixReadOnlyAttr(string file)
    {
      return Regex.Replace(file, ReadOnlyAttrRegex, "", RegexOptions.Multiline);
    }

    const string SwitchCaseLongRegex = @"(case \d+)L:";
    private string FixSwitchCaseLong(string file)
    {
      return Regex.Replace(file, SwitchCaseLongRegex, @"$1", RegexOptions.Multiline);
    }

    private void FixupFile(string filename)
    {
      string file = File.ReadAllText(filename);
      file = FixStructLayout(file);
      file = FixEvents(file);
      file = FixUncheckedHashCode(file);
      file = FixAmbiguousDebugCalls(file);
      file = FixSpecialNameFn(file);
      file = FixBadCtor(file);
      file = FixReadOnlyAttr(file);
      file = FixSwitchCaseLong(file);
      File.WriteAllText(filename, file);
    }


    private void WholeCodebaseFixes(string scriptsDir)
    {
      if (!Directory.Exists(scriptsDir)) return;

      var codeFiles = Directory.EnumerateFiles(scriptsDir, "*.cs", SearchOption.AllDirectories);
      foreach (var file in codeFiles)
      {
        FixupFile(file);
      }
    }

    private void FixUtils(string scriptsDir)
    {
      string filename = Path.Join(scriptsDir, "assembly_utils", "Utils.cs");
      if (!File.Exists(filename)) return;

      string file = File.ReadAllText(filename);
      file = file.Replace(" CompressionLevel.Fastest", " System.IO.Compression.CompressionLevel.Fastest");
      File.WriteAllText(filename, file);
    }

    private void OneOffCodeFixes(string scriptsDir)
    {
      FixUtils(scriptsDir);
    }
  }
}
