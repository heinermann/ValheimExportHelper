using AssetRipper.Core.Logging;

namespace ValheimExportHelper
{
  public abstract class LoggingTrait
  {
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
