using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  class FixCodeFiles : IPostExporter
  {
    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Fixing code files");

      // TODO: Delete folder Microsoft.CSharp
      // TODO: Delete folder Mono.Posix
      // TODO (regex replace in all Assets/MonoScript/**/*.cs files)
    }
  }
}
