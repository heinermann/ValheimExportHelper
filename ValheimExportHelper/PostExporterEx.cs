using AssetRipper.Core.Structure.GameStructure.Platforms;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

/**
 * AssetRipper output log interception notes:
 *
 * For GameDataPath (source path):
 *    General : Attempting to read files from C:\Program Files (x86)\Steam\steamapps\common\Valheim
 *    Import : PC game structure has been found at 'C:\Program Files (x86)\Steam\steamapps\common\Valheim'
 *
 * For ExportRootPath (destination):
 *    General : About to begin export to C:\Users\heine\Valheim_src\patch12\valheim
 *    Export : Attempting to export assets to C:\Users\heine\Valheim_src\patch12\valheim...
 *
 * Finished export:
 *    General : Export Complete!
 */

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
      // ExportRootPath is the base export path, i.e. `<chosenExportDirectory>\valheim`
      ExportRootPath = ripper.Settings.ExportRootPath;
      ProjectRootPath = Path.Join(ExportRootPath, "ExportedProject");
      AssetsPath = Path.Join(ProjectRootPath, "Assets");
      ProjectSettingsPath = Path.Join(ProjectRootPath, "ProjectSettings");

      PlatformGameStructure GameStructure = ripper.GameStructure.MixedStructure ?? ripper.GameStructure.PlatformStructure;
      // GameDataPath is the `valheim_Data` directory i.e. `Steam\steamapps\common\Valheim\valheim_Data`.
      // It should contain a `Plugins` folder.
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
