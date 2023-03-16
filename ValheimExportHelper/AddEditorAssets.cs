namespace ValheimExportHelper
{
  class AddEditorAssets : PostExporterEx
  {
    private string EditorScriptsDir { get; set; }

    public override void Export()
    {
      LogInfo("Adding editor-only assets");

      CreateScriptsDir();
      AddEditorScripts();
      AddBlankScene();
      AddShaderNotes();
    }

    private void CreateScriptsDir()
    {
      EditorScriptsDir = Path.Join(AssetsPath, "Editor", "ValheimExportHelper");
      Directory.CreateDirectory(EditorScriptsDir);
    }

    private void AddEditorScripts()
    {
      File.WriteAllText(Path.Join(EditorScriptsDir, "WorldGeneratorFix.cs"), Resource.WorldGeneratorFix);
      File.WriteAllText(Path.Join(EditorScriptsDir, "CreateAssetBundles.cs"), Resource.AssetBundler);
    }

    private void AddBlankScene()
    {
      LogInfo("Creating blank scene");
      File.WriteAllBytes(Path.Join(AssetsPath, "Scenes", "BlankScene.unity"), Resource.Blank);
    }

    private void AddShaderNotes()
    {
      File.WriteAllBytes(Path.Join(AssetsPath, "Shader", "README.md"), Resource.ShadersReadme);
    }
  }
}
