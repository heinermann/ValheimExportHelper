using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  class FixCodeFiles : PostExporterEx
  {
    private readonly string[] ScriptDirs = new[] { "MonoScript", "Scripts" };

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

      // Script: Dll Export Without Renaming
      TryDelete(Path.Join(scriptsDir, "Microsoft.CSharp.dll"));
      TryDelete(Path.Join(scriptsDir, "Mono.Posix.dll"));
    }

    private void FixupFile(string filename)
    {
      string file = File.ReadAllText(filename);
      file = file.Replace("StructLayout(0", "StructLayout(LayoutKind.Sequential");
      file = file.Replace("StructLayout(2", "StructLayout(LayoutKind.Explicit");
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

    // 1. \r?\n                            Newline (both Windows and Linux)
    // 2. (\s+)\{                          Save the indentation for the brace into a capture group
    // 3. [\s\S]+?                         Everything else (non-greedy, so it stops when it matches the next thing)
    // 4. ^\1\}                            Closing brace with the same indentation as the opening brace
    const string BlockRegex = @"\r?\n(\s+)\{[\s\S]+?^\1\}";

    private void FixSteamworks(string scriptsDir)
    {
      string basePath = Path.Join(scriptsDir, "assembly_steamworks", "Steamworks");

      string callbackSourceFile = Path.Join(basePath, "Callback.cs");
      if (File.Exists(callbackSourceFile))
      {
        string file = File.ReadAllText(callbackSourceFile);
        file = Regex.Replace(file, $"event DispatchDelegate m_Func{BlockRegex}", "event DispatchDelegate m_Func;", RegexOptions.Multiline);
        File.WriteAllText(callbackSourceFile, file);
      }

      string callResultSourceFile = Path.Join(basePath, "CallResult.cs");
      if (File.Exists(callResultSourceFile))
      {
        string file = File.ReadAllText(callResultSourceFile);
        file = Regex.Replace(file, $"event APIDispatchDelegate m_Func{BlockRegex}", "event APIDispatchDelegate m_Func;", RegexOptions.Multiline);
        File.WriteAllText(callResultSourceFile, file);
      }
    }

    private void OneOffCodeFixes(string scriptsDir)
    {
      FixUtils(scriptsDir);
      FixSteamworks(scriptsDir);
    }
  }
}
