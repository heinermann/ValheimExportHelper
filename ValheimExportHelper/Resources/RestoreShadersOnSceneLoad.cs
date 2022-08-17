using System.IO;
using UnityEngine;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class RestoreShadersOnSceneLoad : MonoBehaviour
{
  private static void UnzipFile(string sourceArchive, string targetPath)
  {
    string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    Directory.CreateDirectory(tempDir);
    Directory.CreateDirectory(targetPath);
    ZipFile.ExtractToDirectory(sourceArchive, tempDir);
    foreach (string filename in Directory.EnumerateFiles(tempDir))
    {
      string destpath = Path.Combine(targetPath, Path.GetFileName(filename));
      File.Copy(filename, destpath, overwrite: true);
    }
    Directory.Delete(tempDir, recursive: true);
  }

  [MenuItem("Valheim/Restore Shaders")]
  public static void RestoreOriginalShaders()
  {
    UnzipFile("Assets/Shader_Original.zip", "Assets/Shader");
  }
  static RestoreShadersOnSceneLoad()
  {
    RestoreOriginalShaders();
    EditorSceneManager.sceneOpening += (path, mode) =>
    {
      RestoreOriginalShaders();
    };
  }
}
