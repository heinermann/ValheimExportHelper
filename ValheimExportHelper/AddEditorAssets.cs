using System.IO.Compression;

namespace ValheimExportHelper
{
  class AddEditorAssets : PostExporterEx
  {
    private string ScriptsDir { get; set; }

    public override void Export()
    {
      CreateScriptsDir();
      AddEditorScripts();
      ZipOriginalShaders();
    }

    private void CreateScriptsDir()
    {
      ScriptsDir = Path.Join(CurrentRipper.Settings.AssetsPath, "Scripts");
      Directory.CreateDirectory(ScriptsDir);
    }

    private void AddEditorScripts()
    {
      LogInfo("Adding editor-only scripts");
      File.WriteAllText(Path.Join(ScriptsDir, "Editor.cs"), Resource.Editor);
      File.WriteAllText(Path.Join(ScriptsDir, "ScuffedShaders.cs"), Resource.ScuffedShaders);
    }

    private void ZipOriginalShaders()
    {
      LogInfo("Zipping original shaders to prevent Unity corruption");
      string shaderDir = Path.Join(CurrentRipper.Settings.AssetsPath, "Shader");
      string shaderArchiveName = Path.Join(CurrentRipper.Settings.AssetsPath, "Shader_Original.zip");
      ZipFile.CreateFromDirectory(shaderDir, shaderArchiveName);
    }
  }
}
