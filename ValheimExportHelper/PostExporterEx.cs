using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  internal abstract class PostExporterEx : LoggingTrait, IPostExporter
  {
    public Ripper CurrentRipper { get; set; }

    public void Init(Ripper ripper)
    {
      CurrentRipper = ripper;
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
