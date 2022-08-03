using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;
using YamlDotNet.Serialization;

namespace ValheimExportHelper
{
  class FixUnityProjectSettings : IPostExporter
  {
    const string UnityHeader = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!129 &1";

    private string ProjectSettingsFile { get; set; } = string.Empty;

    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Fixing Unity project settings");

      ProjectSettingsFile = Path.Join(ripper.Settings.ProjectSettingsPath, "ProjectSettings.asset");

      dynamic settings = ReadSettings(ripper);

      //var serializer = new SerializerBuilder().Build();
      //Logger.Warning(serializer.Serialize(settings));

      ApplySettingsChanges(settings);
      WriteSettings(ripper, settings);
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

    private dynamic ReadSettings(Ripper ripper)
    {
      dynamic? yaml = null;
      string? yamlText = null;
      try
      {
        string[] yamlLines = File.ReadAllLines(ProjectSettingsFile);
        yamlLines = yamlLines.Where(line => !line.StartsWith('%') && !line.StartsWith("---")).ToArray();
        yamlText = String.Join('\n', yamlLines);
      }
      catch
      {
        Logger.Error(LogCategory.Plugin, $"[ValheimExportHelper] Failed to open {ProjectSettingsFile}.");
        throw;
      }

      try
      {
        var deserializer = new DeserializerBuilder().Build();
        yaml = deserializer.Deserialize(new StringReader(yamlText));
      }
      catch
      {
        Logger.Error(LogCategory.Plugin, $"[ValheimExportHelper] Failed to deserialize {ProjectSettingsFile}.");
        throw;
      }

      if (yaml == null) throw new NullReferenceException("Failed to modify ProjectSettings.asset");
      return yaml;
    }

    private void WriteSettings(Ripper ripper, dynamic settings)
    {
      var serializer = new SerializerBuilder().Build();
      string result = serializer.Serialize(settings);

      File.WriteAllText(ProjectSettingsFile, $"{UnityHeader}\n{result}");
    }
  }
}
