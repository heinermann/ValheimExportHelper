using YamlDotNet.Serialization;

namespace ValheimExportHelper
{
  public class UnityYaml : LoggingTrait
  {
    public string header = "";
    public dynamic data = null;

    public string filename = null;

    private UnityYaml(string filename)
    {
      this.filename = filename;
    }

    public void Load()
    {
      try
      {
        string[] yamlLines = File.ReadAllLines(filename);
        ParseLines(yamlLines);
      }
      catch
      {
        LogError($"Failed to open {filename}");
        throw;
      }
    }

    public void Save()
    {
      var serializer = new SerializerBuilder().Build();
      string result = serializer.Serialize(data);

      File.WriteAllText(filename, $"{header}\n{result}");
    }

    private bool IsValidYamlLine(string line)
    {
      return !line.StartsWith('%') && !line.StartsWith("---");
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

    private void ParseLines(string[] yamlLines)
    {
      this.header = String.Join('\n', yamlLines.TakeWhile(s => !IsValidYamlLine(s)));

      string content = String.Join('\n', yamlLines.Where(IsValidYamlLine));
      this.data = DeserializeYamlFromText(content);
    }

    public static UnityYaml LoadYaml(string filename)
    {
      UnityYaml result = new UnityYaml(filename);
      result.Load();
      return result;
    }
  }
}
