#if UNITY_EDITOR
using System.IO;
using System.IO.Compression;

public static class UnzipExtension 
{
   public static void UnZipFile(string sourceArchive)
   {
      string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

      Directory.CreateDirectory(tempDir);
      Directory.CreateDirectory("Assets/Shader");
      ZipFile.ExtractToDirectory(sourceArchive, tempDir );
      foreach (string filename in Directory.EnumerateFiles(tempDir))
      {
         File.Copy(filename, Path.Combine("Assets/Shader/", Path.GetFileName(filename)), overwrite: true);
      }
      Directory.Delete(tempDir, recursive: true);
   }
}
#endif