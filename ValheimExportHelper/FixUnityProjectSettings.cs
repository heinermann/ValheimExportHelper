namespace ValheimExportHelper
{
  class FixUnityProjectSettings : PostExporterEx
  {
    private string ProjectSettingsFile { get; set; } = string.Empty;
    private string ProjectPackagePath { get; set; } = string.Empty;

    public override void Export()
    {
      LogInfo("Fixing Unity project settings");

      ProjectSettingsFile = Path.Join(CurrentRipper.Settings.ProjectSettingsPath, "ProjectSettings.asset");
      ProjectPackagePath = Path.Join(CurrentRipper.Settings.ProjectRootPath, "Packages");

      ModifySettings();

      GeneratePackageList();
    }

    private void GeneratePackageList()
    {
      // Generates the package list without version control or postprocessing (these need to be excluded).
      Directory.CreateDirectory(ProjectPackagePath);
      File.WriteAllBytes(Path.Join(ProjectPackagePath, "manifest.json"), Resource.manifest);
    }

    private void ApplySettingsChanges(dynamic settings)
    {
      settings["PlayerSettings"]["m_BuildTargetGraphicsAPIs"] = new[] {
        new {
          m_BuildTarget = "LinuxStandaloneSupport",
          m_APIs = "1100000015000000",
          m_Automatic = 0
        },
        new {
          m_BuildTarget = "WindowsStandaloneSupport",
          m_APIs = "0200000015000000",
          m_Automatic = 0
        }
      };
    }

    private void ModifySettings()
    {
      UnityYaml yaml = UnityYaml.LoadYaml(ProjectSettingsFile);
      ApplySettingsChanges(yaml.data);
      yaml.Save();
    }
  }
}
