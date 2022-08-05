using System.IO.Compression;

namespace ValheimExportHelper
{
  class FixShaders : PostExporterEx
  {
    private string ScriptsDir { get; set; }
    private string ShaderDir { get; set; }

    public override void Export()
    {
      LogInfo("Fixing shaders");

      CreateScriptsDir();
      AddEditorScript();

      ShaderDir = Path.Join(CurrentRipper.Settings.AssetsPath, "Shader");

      DeleteSupportedFreeShaders();
      ZipOriginalShaders();
      ExtractFreeShaders();
    }

    private void CreateScriptsDir()
    {
      ScriptsDir = Path.Join(CurrentRipper.Settings.AssetsPath, "Scripts");
      Directory.CreateDirectory(ScriptsDir);
    }

    private void AddEditorScript()
    {
      File.WriteAllText(Path.Join(ScriptsDir, "Editor.cs"), Resource.Editor);
      File.WriteAllText(Path.Join(ScriptsDir, "ScuffedShaders.cs"), Resource.ScuffedShaders);
    }

    private string GetOldShaderFilename(ZipArchiveEntry entry)
    {
      string shaderName = Path.GetFileNameWithoutExtension(entry.Name);
      return Path.Join(ShaderDir, $"{shaderName}.asset");
    }

    private void DeleteSupportedFreeShaders()
    {
      using (var zip = new ZipArchive(new MemoryStream(Resource.Shaders)))
      {
        zip.Entries.Select(GetOldShaderFilename).ToList().ForEach(TryDelete);
      }
    }

    private void ExtractFreeShaders()
    {
      using (var zip = new ZipArchive(new MemoryStream(Resource.Shaders)))
      {
        zip.ExtractToDirectory(ShaderDir);
      }
    }

    private void ZipOriginalShaders()
    {
      LogInfo("Zipping original shaders to prevent Unity corruption");
      string shaderArchiveName = Path.Join(CurrentRipper.Settings.AssetsPath, "Shader_Original.zip");
      ZipFile.CreateFromDirectory(ShaderDir, shaderArchiveName);
    }

  }
}
