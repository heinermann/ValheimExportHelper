using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  internal abstract class PostExporterEx : IPostExporter
  {
    public Ripper CurrentRipper { get; set; }

    public void Init(Ripper ripper)
    {
      CurrentRipper = ripper;
    }

    void IPostExporter.DoPostExport(Ripper ripper)
    {
      LogInfo($"Running PostExporter module {GetType().Name}");
      Init(ripper);
      Export();
    }

    public abstract void Export();

    public void LogInfo(string text)
    {
      Logger.Info(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }

    public void LogWarn(string text)
    {
      Logger.Warning(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }

    public void LogError(string text)
    {
      Logger.Error(LogCategory.Plugin, $"[{GetType().FullName}] {text}");
    }
  }
}
