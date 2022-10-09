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
      string targetDirPath = Path.Join(CurrentRipper.Settings.ExportRootPath, projectName);

      Directory.Move(CurrentRipper.Settings.ProjectRootPath, targetDirPath);
    }

    private string ExtractDocumentValue(string document, string variable)
    {
      Match match = Regex.Match(document, $@"int {variable} = (\d+);", RegexOptions.Multiline);
      if (!match.Success)
      {
        LogError($"Failed to retrieve Version information for {variable}");
        return "0";
      }
      return match.Groups[1].Value;
    }

    private static readonly string[] ScriptDirs = new[] { "MonoScript", "Scripts" };

    private string GetVersionFile()
    {
      foreach (string dir in ScriptDirs)
      {
        string versionSourceFilename = Path.Join(CurrentRipper.Settings.AssetsPath, dir, "assembly_valheim", "Version.cs");
        if (File.Exists(versionSourceFilename)) return File.ReadAllText(versionSourceFilename);
      }
      return null;
    }

    private string GetVersionString()
    {
      string versionSource = GetVersionFile();
      if (versionSource == null) return "DllExport";

      string major = ExtractDocumentValue(versionSource, "m_major");
      string minor = ExtractDocumentValue(versionSource, "m_minor");
      string patch = ExtractDocumentValue(versionSource, "m_patch");

      return $"{major}.{minor}.{patch}";
    }
  }
}
