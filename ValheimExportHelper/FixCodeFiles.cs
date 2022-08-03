using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;
using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  class FixCodeFiles : IPostExporter
  {
    private Ripper? Ripper { get; set; }
    private string? MonoScriptDir { get; set; }

    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Fixing code files");

      Ripper = ripper;
      MonoScriptDir = Path.Join(ripper.Settings.AssetsPath, "MonoScript");

      DeleteStandardLibraries();
      WholeCodebaseFixes();
      OneOffCodeFixes();
    }

    private void TryDelete(string filename)
    {
      if (Directory.Exists(filename))
      {
        Directory.Delete(filename, true);
      }
      else if (File.Exists(filename))
      {
        File.Delete(filename);
      }
    }

    private void DeleteStandardLibraries()
    {
      // Script: Decompiled
      TryDelete(Path.Join(MonoScriptDir, "Microsoft.CSharp"));
      TryDelete(Path.Join(MonoScriptDir, "Mono.Posix"));

      // Script: Dll Export Without Renaming
      TryDelete(Path.Join(MonoScriptDir, "Microsoft.CSharp.dll"));
      TryDelete(Path.Join(MonoScriptDir, "Mono.Posix.dll"));
    }

    // TODO (regex replace in all Assets/MonoScript/**/*.cs files)

    private void FixupFile(string filename)
    {
      string file = File.ReadAllText(filename);
      file = file.Replace("StructLayout(0", "StructLayout(LayoutKind.Sequential");
      file = file.Replace("StructLayout(2", "StructLayout(LayoutKind.Explicit");
      File.WriteAllText(filename, file);
    }

    private void WholeCodebaseFixes()
    {
      foreach (var file in Directory.EnumerateFiles(MonoScriptDir, "*.cs", SearchOption.AllDirectories))
      {
        FixupFile(file);
      }
    }

    private void FixUtils()
    {
      string filename = Path.Join(MonoScriptDir, "assembly_utils", "Utils.cs");
      if (!File.Exists(filename)) return;

      string file = File.ReadAllText(filename);
      file = file.Replace(" CompressionLevel.Fastest", " System.IO.Compression.CompressionLevel.Fastest");
      File.WriteAllText(filename, file);
    }

    private void FixSteamworks()
    {
      string basePath = Path.Join(MonoScriptDir, "assembly_steamworks", "Steamworks");

      string callbackSourceFile = Path.Join(basePath, "Callback.cs");
      if (File.Exists(callbackSourceFile))
      {
        string file = File.ReadAllText(callbackSourceFile);

        // 1. event DispatchDelegate m_Func    Match the declaration of m_Func
        // 2. \r?\n                            Newline (both Windows and Linux)
        // 3. (\s+)\{                          Save the indentation for the brace into a capture group
        // 4. [\s\S]+?                         Everything else (non-greedy, so it stops when it matches the next thing)
        // 5. ^\1\}                            Closing brace with the same indentation as the opening brace
        file = Regex.Replace(file, @"event DispatchDelegate m_Func\r?\n(\s+)\{[\s\S]+?^\1\}", "event DispatchDelegate m_Func;", RegexOptions.Multiline);
        File.WriteAllText(callbackSourceFile, file);
      }

      string callResultSourceFile = Path.Join(basePath, "CallResult.cs");
      if (File.Exists(callResultSourceFile))
      {
        string file = File.ReadAllText(callResultSourceFile);

        // Similar regex as above
        file = Regex.Replace(file, @"event APIDispatchDelegate m_Func\r?\n(\s+)\{[\s\S]+?^\1\}", "event APIDispatchDelegate m_Func;", RegexOptions.Multiline);
        File.WriteAllText(callResultSourceFile, file);
      }
    }

    private void OneOffCodeFixes()
    {
      FixUtils();
      FixSteamworks();
    }
  }
}
