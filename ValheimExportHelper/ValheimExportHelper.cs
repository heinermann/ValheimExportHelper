using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ValheimExportHelper
{
  class Logger : LoggingTrait { }

  public static class ValheimExportHelper
  {
    private static Ripper ripper = new Ripper();
    private static Regex importRegex = new Regex(@"Import : PC game structure has been found at '(.*)'");
    private static Regex exportRegex = new Regex(@"Export : Attempting to export assets to (.*)\.\.\.");

    private static Logger log = new Logger();

    static void Main(string[] args)
    {
      Console.WriteLine(ValheimAsciiLogo);
      LaunchAssetRipper();
    }

    static void LaunchAssetRipper()
    {
      Process process = new Process();
      process.EnableRaisingEvents = true;

      process.OutputDataReceived += new DataReceivedEventHandler(AssetRipperOutput);
      process.ErrorDataReceived += new DataReceivedEventHandler(AssetRipperError);
      process.Exited += new EventHandler(AssetRipperClosed);

      process.StartInfo.FileName = "AssetRipper.exe";
      //process.StartInfo.Arguments = "";
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();

      process.WaitForExit();
    }

    static void AssetRipperClosed(object sender, EventArgs e)
    {
      log.LogInfo("AssetRipper was closed.");
    }

    static void AssetRipperError(object sender, DataReceivedEventArgs e)
    {
      if (e.Data == null) return;
      Console.BackgroundColor = ConsoleColor.Red;
      Console.WriteLine($"{e.Data}");
      Console.ResetColor();
    }

    static void AssetRipperOutput(object sender, DataReceivedEventArgs e)
    {
      if (e.Data == null) return;
      Console.WriteLine($"{e.Data}");

      var importMatch = importRegex.Match(e.Data);
      var exportMatch = exportRegex.Match(e.Data);

      if (importMatch.Success)
      {
        ripper.GameDataPath = importMatch.Groups[1].Value + "\\valheim_Data";
        log.LogInfo("Found GameDataPath.");
      }
      else if (exportMatch.Success)
      {
        ripper.ExportRootPath = exportMatch.Groups[1].Value;
        log.LogInfo("Found ExportRootPath.");
      }
      else if (e.Data == "General : Export Complete!")
      {
        OnFinishExporting();
      }
    }

    static void OnFinishExporting()
    {
      var stages = new List<PostExporterEx>()
      {
        new AddEditorAssets(),
        new AddPostProcessingPackage(),
        new FixCodeFiles(),
        new FixCursor(),
        new FixPlugins(),
        new FixUnityProjectSettings(),
        new FixWAVs(),
        new RenameExportDir() // THIS MUST BE LAST
      };

      foreach (PostExporterEx stage in stages)
      {
        stage.DoPostExport(ripper);
      }
      log.LogInfo("Finished.");
    }

    // Created and modified from https://asciiart.club/
    const string ValheimAsciiLogo = @"
     'N@@N!``       '|@%M*`
       '] .         .WL   .,;,,   :@@mm~    ~g=~  ,;vy,,;;,, .;@,.k@@w- mmr  j@@@w
        '%/g,       i|'   `L]y'    '%\      j,:   '[[* '%@|;jF**T !RU  ;j%g, /&%L`
         'n]M     .)@L`   ,.'nu    .k:     'j$`   '[H  '|Rr`  .p  !N!  ;,r%@@$|K|
         ']@@r    !]@     \: |];   'OY      j|r  ;gMk  '|K,,=!~|| ;N|  ;$r' *`|@~
          '%#W,   [[     Cpx,*|k.  :n|      ]%UY[^`LK  '[[!`    ` *R|  |%U`   |g|
           `U]!  ,,``   /|=``  pp  'j@ .:r  ]|`   :F@   [[   zi@; ,J|  ;jL    [%~
            ]=N!.|@=   ,|k    '])' :\L'~:=|\]),   'F|   [!;;ij||* :lM  j@k   .[%y
            '||L`|!  ,(l*'     '```j``     '*^^   ~~-'  *~*       ***``'**`  '*]*`
            `j|%F:`  h|j$@@/::;ssssq/vFwgpgg@gps;Wgy=\w=wqw<#WW@zgpgwggwwgggg@@%|{
             'l|,`  ']M%****`*w%%~^'``''*H**TTT**q|)r*``````````*'**w%%*TTTTTT%MM`
               V                                 '|`
";
  }
}