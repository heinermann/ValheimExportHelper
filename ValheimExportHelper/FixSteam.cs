using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  class FixSteam : IPostExporter
  {
    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Fixing Steam");

      CopyAppId(ripper);
      CopyPlugins(ripper);
    }

    private void CopyAppId(Ripper ripper)
    {
      string rootPath = ripper.GameStructure.MixedStructure?.RootPath ?? ripper.GameStructure.PlatformStructure?.RootPath;
      string srcPath = Path.Join(rootPath, "steam_appid.txt");
      string dstPath = Path.Join(ripper.Settings.ProjectRootPath, "steam_appid.txt");
      File.Copy(srcPath, dstPath, overwrite: true);
    }

    private void CopyPlugins(Ripper ripper)
    {
      string dstPath = Path.Join(ripper.Settings.AssetsPath, "Plugins", "x86_64");
      Directory.CreateDirectory(dstPath);
      
      string gameDataPath = ripper.GameStructure.MixedStructure?.GameDataPath ?? ripper.GameStructure.PlatformStructure?.GameDataPath;
      string srcPath = Path.Join(gameDataPath, "Plugins", "x86_64", "steam_api64.dll");
      File.Copy(srcPath, Path.Join(dstPath, "steam_api64.dll"), overwrite: true);
    }
  }
}
