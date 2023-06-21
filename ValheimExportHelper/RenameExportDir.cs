using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  /**
   * Notes: Originally supposed to create a symbolic link, but Windows does not give users symlink permissions by default (citing "security risks").
   */
  class RenameExportDir : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Renaming project directory");

      string projectName = $"Valheim {GetVersionString()} - {DateTime.Now:yyyy-MM-dd}";
      string targetDirPath = Path.Join(ExportRootPath, projectName);

      Directory.Move(ProjectRootPath, targetDirPath);
    }

    private string ExtractDocumentValue(string document, string variable)
    {
      Match match = Regex.Match(document, $@"int {variable} = (\d+);", RegexOptions.Multiline);
      if (match.Success)
      {
        return match.Groups[1].Value;
      }
      return null;
    }

    private string ExtractOldVersionString(string document)
    {
      string major = ExtractDocumentValue(document, "m_major");
      string minor = ExtractDocumentValue(document, "m_minor");
      string patch = ExtractDocumentValue(document, "m_patch");

      if (major == null || minor == null || patch == null) return null;
      return $"{major}.{minor}.{patch}";
    }

    private string ExtractNewVersionString(string document)
    {
      Match match = Regex.Match(document, @"GameVersion CurrentVersion .*new GameVersion\((\d+), (\d+), (\d+)\);", RegexOptions.Multiline);
      if (match.Success)
      {
        var grp = match.Groups;
        return $"{grp[1].Value}.{grp[2].Value}.{grp[3].Value}";
      }
      return null;
    }

    private static readonly string[] ScriptDirs = new[] { "MonoScript", "Scripts" };

    private string GetVersionFile()
    {
      foreach (string dir in ScriptDirs)
      {
        string versionSourceFilename = Path.Join(AssetsPath, dir, "assembly_valheim", "Version.cs");
        if (File.Exists(versionSourceFilename)) return File.ReadAllText(versionSourceFilename);
      }
      return null;
    }

    private string GetVersionString()
    {
      string versionSource = GetVersionFile();
      if (versionSource == null) return "DllExport";

      string result = ExtractOldVersionString(versionSource);
      if (result == null)
      {
        result = ExtractNewVersionString(versionSource);
        if (result == null)
        {
          LogError("Unable to identify version.");
          result = "unknown";
        }
      }
      return result;
    }
  }
}
