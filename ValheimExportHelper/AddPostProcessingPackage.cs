using System.IO.Compression;

namespace ValheimExportHelper
{
  class AddPostProcessingPackage : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Adding a deprecated version of PostProcessing");

      using (var zip = new ZipArchive(new MemoryStream(Resource.PostProcessing)))
      {
        zip.ExtractToDirectory(AssetsPath);
      }
    }
  }
}
