using YamlDotNet.Serialization;

namespace ValheimExportHelper
{
  class FixUnityProjectSettings : PostExporterEx
  {
    const string UnityHeader = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!129 &1";

    private string ProjectSettingsFile { get; set; } = string.Empty;
    private string ProjectPackagePath { get; set; } = string.Empty;

    public override void Export()
    {
      LogInfo("Fixing Unity project settings");

      ProjectSettingsFile = Path.Join(CurrentRipper.Settings.ProjectSettingsPath, "ProjectSettings.asset");
      ProjectPackagePath = Path.Join(CurrentRipper.Settings.ProjectRootPath, "Packages");

      dynamic settings = ReadSettings();
      ApplySettingsChanges(settings);
      WriteSettings(settings);
      
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

    private bool IsValidYamlLine(string line)
    {
      return !line.StartsWith('%') && !line.StartsWith("---");
    }

    private string ReadYamlFileAsText(string fileName)
    {
      try
      {
        string[] yamlLines = File.ReadAllLines(fileName)
          .Where(IsValidYamlLine)
          .ToArray();
        return String.Join('\n', yamlLines);
      }
      catch
      {
        LogError($"Failed to open {ProjectSettingsFile}");
        throw;
      }
    }

    private dynamic DeserializeYamlFromText(string yamlText)
    {
      try
      {
        var deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize(new StringReader(yamlText));
      }
      catch
      {
        LogError($"Failed to deserialize {ProjectSettingsFile}");
        throw;
      }
    }

    private dynamic ReadSettings()
    {
      string yamlText = ReadYamlFileAsText(ProjectSettingsFile);
      dynamic settings = DeserializeYamlFromText(yamlText);

      if (settings == null) throw new NullReferenceException("Failed to modify ProjectSettings.asset");
      return settings;
    }

    private void WriteSettings(dynamic settings)
    {
      var serializer = new SerializerBuilder().Build();
      string result = serializer.Serialize(settings);

      File.WriteAllText(ProjectSettingsFile, $"{UnityHeader}\n{result}");
    }
  }
}
