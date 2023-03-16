using AssetRipper.Core.Structure.GameStructure.Platforms;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  internal abstract class PostExporterEx : LoggingTrait, IPostExporter
  {
    protected string ProjectRootPath;
    protected string AssetsPath;
    protected string ExportRootPath;
    protected string GameDataPath;
    protected string ProjectSettingsPath;

    public void Init(Ripper ripper)
    {
      ProjectRootPath = ripper.Settings.ProjectRootPath;
      AssetsPath = ripper.Settings.AssetsPath;
      ExportRootPath = ripper.Settings.ExportRootPath;
      ProjectSettingsPath = ripper.Settings.ProjectSettingsPath;

      PlatformGameStructure GameStructure = ripper.GameStructure.MixedStructure ?? ripper.GameStructure.PlatformStructure;
      GameDataPath = GameStructure.GameDataPath;
    }

    void IPostExporter.DoPostExport(Ripper ripper)
    {
      Init(ripper);
      Export();
    }

    public abstract void Export();

    public void TryDelete(string filename)
    {
      if (Directory.Exists(filename)) Directory.Delete(filename, true);
      else if (File.Exists(filename)) File.Delete(filename);
    }
  }
}
