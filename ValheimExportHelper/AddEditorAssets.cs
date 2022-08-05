using System.IO.Compression;

namespace ValheimExportHelper
{
  class AddEditorAssets : PostExporterEx
  {
    private string ScriptsDir { get; set; }

    public override void Export()
    {
      LogInfo("Adding editor-only assets");

      CreateScriptsDir();
      AddEditorScripts();
      AddBlankScene();
    }

    private void CreateScriptsDir()
    {
      ScriptsDir = Path.Join(CurrentRipper.Settings.AssetsPath, "Scripts");
      Directory.CreateDirectory(ScriptsDir);
    }

    private void AddEditorScripts()
    {
      File.WriteAllText(Path.Join(ScriptsDir, "Editor.cs"), Resource.Editor);
    }

    private void AddBlankScene()
    {
      LogInfo("Creating blank scene");
      File.WriteAllBytes(Path.Join(CurrentRipper.Settings.AssetsPath, "Scenes", "BlankScene.unity"), Resource.Blank);
    }
  }
}
