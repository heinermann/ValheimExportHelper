#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Note: This script deletes and re-extracts the original shaders.
// This is because Unity periodically corrupts the shaders (and then they stop working).
[InitializeOnLoad]
public class ScuffedShaders : MonoBehaviour
{
  static void RestoreOriginalShaders() {
    string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

    Directory.CreateDirectory(tempDir);
    Directory.CreateDirectory("Assets/Shader");

    ZipFile.ExtractToDirectory("Assets/Shader_Original.zip", tempDir);

    foreach (string filename in Directory.EnumerateFiles(tempDir))
    {
      File.Copy(filename, Path.Combine("Assets/Shader/", Path.GetFileName(filename)), overwrite: true);
    }
    Directory.Delete(tempDir, recursive: true);
  }

  static ScuffedShaders() {
    RestoreOriginalShaders();

    EditorSceneManager.sceneOpening += (path, mode) => {
      RestoreOriginalShaders();
    };
  }
}
#endif
