using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  class AddEditorAssets : IPostExporter
  {
    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Adding additional editor assets");

      string scriptsDir = Path.Combine(ripper.Settings.AssetsPath, "Scripts");
      Directory.CreateDirectory(scriptsDir);
      File.WriteAllText(Path.Combine(scriptsDir, "Editor.cs"), Resource.Editor);
    }
  }
}
