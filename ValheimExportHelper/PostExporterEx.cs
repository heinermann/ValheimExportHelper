using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;
using YamlDotNet.Serialization;

namespace ValheimExportHelper
{
  internal abstract class PostExporterEx : IPostExporter
  {
    public Ripper CurrentRipper { get; set; }
    private bool notified = false;

    public void Init(Ripper ripper)
    {
      CurrentRipper = ripper;
    }

    void IPostExporter.DoPostExport(Ripper ripper)
    {
      Init(ripper);
      Export();
      if (!notified) LogInfo($"Ran PostExporter module {GetType().Name}");
    }

    public abstract void Export();

    public void LogInfo(string text)
    {
      notified = true;
      Logger.Info(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }

    public void LogWarn(string text)
    {
      Logger.Warning(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }

    public void LogError(string text)
    {
      Logger.Error(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }

    public void TryDelete(string filename)
    {
      if (Directory.Exists(filename)) Directory.Delete(filename, true);
      else if (File.Exists(filename)) File.Delete(filename);
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
        LogError($"Failed to open {fileName}");
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
        LogError($"Failed to deserialize yaml content:\n{yamlText}");
        throw;
      }
    }

    public dynamic ReadYamlFile(string filename)
    {
      string yamlText = ReadYamlFileAsText(filename);
      return DeserializeYamlFromText(yamlText);
    }

    public void WriteYamlFile(string filename, dynamic yaml)
    {
      var serializer = new SerializerBuilder().Build();
      string result = serializer.Serialize(yaml);

      File.WriteAllText(filename, result);
    }
  }
}
