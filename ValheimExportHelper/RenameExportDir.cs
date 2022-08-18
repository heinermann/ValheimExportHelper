using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  class RenameExportDir : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Renaming project directory");

      string projectName = $"Valheim {GetVersionString()} - {DateTime.Now:yyyy-MM-dd}";
      string targetDirPath = Path.Join(CurrentRipper.Settings.ExportRootPath, projectName);

      //Directory.CreateSymbolicLink(targetDirPath, CurrentRipper.Settings.ProjectRootPath); // wtf? not working

      File.WriteAllText(Path.Join(CurrentRipper.Settings.ProjectSettingsPath, "ProjectVersion.txt"), "m_EditorVersion: 2020.3.33f1");
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

    private string GetVersionString()
    {
      string versionSourceFilename = Path.Join(CurrentRipper.Settings.AssetsPath, "MonoScript", "assembly_valheim", "Version.cs");

      if (!File.Exists(versionSourceFilename)) return "DllExport";

      string versionSource = File.ReadAllText(versionSourceFilename);

      string major = ExtractDocumentValue(versionSource, "m_major");
      string minor = ExtractDocumentValue(versionSource, "m_minor");
      string patch = ExtractDocumentValue(versionSource, "m_patch");

      return $"{major}.{minor}.{patch}";
    }
  }
}
