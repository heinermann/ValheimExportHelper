using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;
using System.IO.Compression;

namespace ValheimExportHelper
{
  class AddPostProcessingPackage : IPostExporter
  {
    public void DoPostExport(Ripper ripper)
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Adding old PostProcessing package");

      using (var zip = new ZipArchive(new MemoryStream(Resource.PostProcessing)))
      {
        zip.ExtractToDirectory(ripper.Settings.AssetsPath);
      }
    }
  }
}
