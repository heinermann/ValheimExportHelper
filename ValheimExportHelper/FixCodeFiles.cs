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

    private void DeleteStandardLibraries(string scriptsDir)
    {
      LogInfo("Deleting standard libraries");

      // Script: Decompiled
      TryDelete(Path.Join(scriptsDir, "Microsoft.CSharp"));
      TryDelete(Path.Join(scriptsDir, "Mono.Posix"));
      TryDelete(Path.Join(scriptsDir, "XGamingRuntime"));

      // Script: Dll Export Without Renaming
      TryDelete(Path.Join(scriptsDir, "Microsoft.CSharp.dll"));
      TryDelete(Path.Join(scriptsDir, "Mono.Posix.dll"));
      TryDelete(Path.Join(scriptsDir, "XGamingRuntime.dll"));
    }

    private string FixStructLayout(string file)
    {
      file = file.Replace("StructLayout(0", "StructLayout(LayoutKind.Sequential");
      file = file.Replace("StructLayout(2", "StructLayout(LayoutKind.Explicit");
      return file;
    }

    /**
     * 1. ^(\s.*\bevent [\w<>.]+ \w+)\r?\n                    Event definition
     * 2. (\s+)\{\r?\n                                        First open brace + spacing capture
     * 3. (\2\s+)\[CompilerGenerated\]\r?\n                   First CompilerGenerated tag + spacing capture
     * 4. \3add\r?\n                                          "add" declaration
     * 5. \3\{[\w\W]+?\r?\n\3\}\r?\n                          "add" block capture
     * 6. \3\[CompilerGenerated\]\r?\n                        Second CompilerGenerated tag
     * 7. \3remove\r?\n                                       "remove" declaration
     * 8. \3\{[\w\W]+?\r?\n\3\}\r?\n                          "remove" block capture
     * 9. \2}                                                 Closing first brace.
     */
    const string EventRegex = @"^(\s.*\bevent [\w<>.]+ \w+)\r?\n(\s+)\{\r?\n(\2\s+)\[CompilerGenerated\]\r?\n\3add\r?\n\3\{[\w\W]+?\r?\n\3\}\r?\n\3\[CompilerGenerated\]\r?\n\3remove\r?\n\3\{[\w\W]+?\r?\n\3\}\r?\n\2\}";
    private string FixEvents(string file)
    {
      return Regex.Replace(file, EventRegex, @"$1;", RegexOptions.Multiline);
    }

    const string UncheckedRegex = @"override int GetHashCode\(\)\r?\n\s+\{\r?\n\s+return ";
    private string FixUncheckedHashCode(string file)
    {
      return Regex.Replace(file, UncheckedRegex, @"$0 unchecked ", RegexOptions.Multiline);
    }

    private void FixupFile(string filename)
    {
      string file = File.ReadAllText(filename);
      file = FixStructLayout(file);
      file = FixEvents(file);
      file = FixUncheckedHashCode(file);
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
