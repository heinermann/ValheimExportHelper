namespace ValheimExportHelper
{
  class FixWAVs : PostExporterEx
  {
    public override void Export()
    {
      LogInfo("Fixing corrupted WAV files");

      string boilLoopWavPath = Path.Join(CurrentRipper.Settings.AssetsPath, "AudioClip", "Water_BoilLoop.wav");
      FixWAVFile(boilLoopWavPath);
    }

    private void FixWAVFile(string filename)
    {
      byte[] wavFile = File.ReadAllBytes(filename);

      int len = wavFile.Length;
      BitConverter.GetBytes(len - 0x08).CopyTo(wavFile, 0x04);
      BitConverter.GetBytes(len - 0x2E).CopyTo(wavFile, 0x2A);

      File.WriteAllBytes(filename, wavFile);
    }
  }
}
