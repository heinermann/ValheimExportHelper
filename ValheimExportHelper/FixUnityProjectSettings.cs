namespace ValheimExportHelper
{
  class FixUnityProjectSettings : PostExporterEx
  {
    private string ProjectPackagePath { get; set; } = string.Empty;

    public override void Export()
    {
      LogInfo("Fixing Unity project settings");

      ProjectPackagePath = Path.Join(CurrentRipper.Settings.ProjectRootPath, "Packages");

      AddVulkanSupport();
      DisableMotionBlur();

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

    private void AddVulkanSupport()
    {
      string filename = Path.Join(CurrentRipper.Settings.ProjectSettingsPath, "ProjectSettings.asset");
      UnityYaml yaml = UnityYaml.LoadYaml(filename);
      ApplySettingsChanges(yaml.Data);
      yaml.Save();
    }

    private void DisableMotionBlur()
    {
      string filename = Path.Join(CurrentRipper.Settings.AssetsPath, "MonoBehaviour", "ingame.asset");
      UnityYaml yaml = UnityYaml.LoadYaml(filename);
      yaml.Data["MonoBehaviour"]["motionBlur"]["m_Enabled"] = "0";
      yaml.Save();
    }

  }
}
