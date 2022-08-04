using AssetRipper.Core.Structure.GameStructure.Platforms;

namespace ValheimExportHelper
{
  class FixSteam : PostExporterEx
  {
    PlatformGameStructure GameStructure { get; set; }

    public override void Export()
    {
      LogInfo("Adding Steam library dependencies");

      GameStructure = CurrentRipper.GameStructure.MixedStructure ?? CurrentRipper.GameStructure.PlatformStructure;

      CopyAppId();
      CopyPlugins();
    }

    private void CopyAppId()
    {
      string srcFilename = Path.Join(GameStructure.RootPath, "steam_appid.txt");
      string dstFilename = Path.Join(CurrentRipper.Settings.ProjectRootPath, "steam_appid.txt");
      File.Copy(srcFilename, dstFilename, overwrite: true);
    }

    private void CopyPlugins()
    {
      string dstPath = Path.Join(CurrentRipper.Settings.AssetsPath, "Plugins", "x86_64");
      Directory.CreateDirectory(dstPath);
      
      string srcFilename = Path.Join(GameStructure.GameDataPath, "Plugins", "x86_64", "steam_api64.dll");
      string dstFilename = Path.Join(dstPath, "steam_api64.dll");
      File.Copy(srcFilename, dstFilename, overwrite: true);
    }
  }
}
