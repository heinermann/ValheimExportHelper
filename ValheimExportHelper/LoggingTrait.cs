namespace ValheimExportHelper
{
  public abstract class LoggingTrait
  {
    public void LogInfo(string text)
    {
      Console.WriteLine($"[{GetType().FullName}] {text}");
    }

    public void LogWarn(string text)
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine($"[WARN] [{GetType().FullName}] {text}");
      Console.ResetColor();
    }

    public void LogError(string text)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine($"[ERROR] [{GetType().FullName}] {text}");
      Console.ResetColor();
    }
  }
}
