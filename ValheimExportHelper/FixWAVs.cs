namespace ValheimExportHelper
{
  class FixWAVs : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Fixing corrupted WAV files");
      FixAllWAVFiles();
    }

    private void FixWAVFile(string filename)
    {
      if (File.Exists(filename))
      {
        byte[] wavFile = File.ReadAllBytes(filename);
        int len = wavFile.Length;

        if (len < 44) return; // Assume it's unfixable
        uint riff_size = BitConverter.ToUInt32(wavFile, 0x04);
        uint data_size = BitConverter.ToUInt32(wavFile, 0x2A);

        if (riff_size != 0 && data_size != 0) return; // Assume it doesn't need fixing

        BitConverter.GetBytes(len - 0x08).CopyTo(wavFile, 0x04);
        BitConverter.GetBytes(len - 0x2E).CopyTo(wavFile, 0x2A);

        File.WriteAllBytes(filename, wavFile);
        LogInfo($"Fixing WAV: {filename}");
      }
    }

    private void FixAllWAVFiles()
    {
      var wavFiles = Directory.EnumerateFiles(AssetsPath, "*.wav", SearchOption.AllDirectories);
      foreach (var file in wavFiles)
      {
        FixWAVFile(file);
      }
    }
  }
}
