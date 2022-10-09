using AssetRipper.Core.Structure.GameStructure.Platforms;

namespace ValheimExportHelper
{
  class FixPlugins : PostExporterEx
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
      string pluginsPath = Path.Join(GameStructure.GameDataPath, "Plugins");
      string dstPath = Path.Join(CurrentRipper.Settings.AssetsPath, "Plugins");

      // Recreate subdirectory tree
      Directory.CreateDirectory(dstPath);
      var directories = Directory.GetDirectories(pluginsPath, "*", SearchOption.AllDirectories);
      foreach (string dir in directories)
      {
        string dirToCreate = dir.Replace(pluginsPath, dstPath);
        Directory.CreateDirectory(dirToCreate);
      }

      // Copy all files recursively
      var pluginFiles = Directory.EnumerateFiles(pluginsPath, "*.*", SearchOption.AllDirectories);
      foreach (var pluginFile in pluginFiles)
      {
        string dstFile = pluginFile.Replace(pluginsPath, dstPath);
        File.Copy(pluginFile, dstFile, overwrite: true);
      }
    }
  }
}
