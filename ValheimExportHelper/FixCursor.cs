namespace ValheimExportHelper
{
  class FixCursor : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Fixing cursor texture type");

      string cursorMetaFile = Path.Join(CurrentRipper.Settings.AssetsPath, "Texture2D", "cursor.png.meta");

      FixCursorMetadata(cursorMetaFile);
    }

    private void FixCursorMetadata(string filename)
    {
      UnityYaml yaml = UnityYaml.LoadYaml(filename);
      yaml.Data["TextureImporter"]["textureType"] = "7";
      yaml.Save();
    }
  }
}
