using AssetRipper.Core.Logging;
using AssetRipper.Library;
using AssetRipper.Library.Exporters;

namespace ValheimExportHelper
{
  public class ValheimExportHelper : PluginBase
  {
    public override string Name => "ValheimExportHelper";

    public override void Initialize()
    {
      Logger.Info(LogCategory.Plugin, "[ValheimExportHelper] Initializing... Waiting to detect game.");
      CurrentRipper.OnFinishLoadingGameStructure += OnFinishLoadingGameStructure;
    }

    private bool IsValheim()
    {
      return CurrentRipper.GameStructure.Name != null && CurrentRipper.GameStructure.Name.Contains("valheim", StringComparison.InvariantCultureIgnoreCase);
    }

    private void OnFinishLoadingGameStructure()
    {
      if (!IsValheim())
      {
        Logger.Info("[ValheimExportHelper] Game is NOT Valheim, we do nothing");
        return;
      }

      Logger.Info("[ValheimExportHelper] Detected as Valheim");
      Logger.Info(ValheimAsciiLogo);

      new List<IPostExporter>()
      {
        new FixUnityProjectSettings(),
        new AddPostProcessingPackage(),
        new AddEditorAssets(),
        new FixCodeFiles(),
        new FixSteam()
      }.ForEach(CurrentRipper.AddPostExporter);
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