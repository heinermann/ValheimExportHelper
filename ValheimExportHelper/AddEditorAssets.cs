namespace ValheimExportHelper
{
  class AddEditorAssets : PostExporterEx
  {
    private string ScriptsDir { get; set; }
    private string EditorScriptsDir { get; set; }

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
      EditorScriptsDir = Path.Join(ScriptsDir, "Editor");
      Directory.CreateDirectory(EditorScriptsDir);
    }

    private void AddEditorScripts()
    {
      File.WriteAllBytes(Path.Join(CurrentRipper.Settings.AssetsPath, "csc.rsp"), Resource.csc);
      File.WriteAllText(Path.Join(EditorScriptsDir, "Editor.cs"), Resource.Editor);
      File.WriteAllText(Path.Join(EditorScriptsDir, "CreateAssetBundles.cs"), Resource.AssetBundler);
      File.WriteAllText(Path.Join(EditorScriptsDir, "RestoreShadersOnSceneLoad.cs"), Resource.ScuffedShaders);
    }

    private void AddBlankScene()
    {
      LogInfo("Creating blank scene");
      File.WriteAllBytes(Path.Join(CurrentRipper.Settings.AssetsPath, "Scenes", "BlankScene.unity"), Resource.Blank);
    }
  }
}
