using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;
using System.IO.Compression;

namespace ValheimExportHelper
{
  class AddEditorAssets : IPostExporter
  {
    private Ripper? Ripper { get; set; }
    private string ScriptsDir { get; set; } = String.Empty;

    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Adding additional editor assets");
      
      Ripper = ripper;
      CreateScriptsDir();
      AddEditorScripts();
      ZipOriginalShaders();
    }

    private void CreateScriptsDir()
    {
      string scriptsDir = Path.Join(Ripper.Settings.AssetsPath, "Scripts");
      Directory.CreateDirectory(scriptsDir);
      ScriptsDir = scriptsDir;
    }

    private void AddEditorScripts()
    {
      File.WriteAllText(Path.Join(ScriptsDir, "Editor.cs"), Resource.Editor);
      File.WriteAllText(Path.Join(ScriptsDir, "ScuffedShaders.cs"), Resource.ScuffedShaders);
    }

    private void ZipOriginalShaders()
    {
      string shaderDir = Path.Join(Ripper.Settings.AssetsPath, "Shader");
      string shaderArchiveName = Path.Join(Ripper.Settings.AssetsPath, "Shader_Original.zip");
      ZipFile.CreateFromDirectory(shaderDir, shaderArchiveName);
    }
  }
}
