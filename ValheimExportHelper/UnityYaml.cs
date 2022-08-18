using YamlDotNet.Serialization;

namespace ValheimExportHelper
{
  public class UnityYaml : LoggingTrait
  {
    public string Header { get; private set; } = "";
    public dynamic Data { get; private set; }
    public string Filename { get; private set; }


    private UnityYaml(string filename)
    {
      this.Filename = filename;
    }

    public void Load()
    {
      try
      {
        string[] yamlLines = File.ReadAllLines(Filename);
        ParseLines(yamlLines);
      }
      catch
      {
        LogError($"Failed to open {Filename}");
        throw;
      }
    }

    public void Save()
    {
      var serializer = new SerializerBuilder().Build();
      string result = serializer.Serialize(Data);

      File.WriteAllText(Filename, $"{Header}\n{result}");
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
      this.Header = String.Join('\n', yamlLines.TakeWhile(s => !IsValidYamlLine(s)));

      string content = String.Join('\n', yamlLines.Where(IsValidYamlLine));
      this.Data = DeserializeYamlFromText(content);
    }

    public static UnityYaml LoadYaml(string filename)
    {
      UnityYaml result = new UnityYaml(filename);
      result.Load();
      return result;
    }
  }
}
