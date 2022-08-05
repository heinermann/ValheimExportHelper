namespace ValheimExportHelper
{
  class FixCursor : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Fixing cursor texture type");

      string cursorMetaFile = Path.Join(CurrentRipper.Settings.AssetsPath, "Texture2D", "cursor.png.meta");
      
      dynamic yaml = ReadYamlFile(cursorMetaFile);
      yaml["TextureImporter"]["textureType"] = "7";
      WriteYamlFile(cursorMetaFile, yaml);
    }
  }
}
