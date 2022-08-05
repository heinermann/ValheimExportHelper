using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public class RestoreShadersOnSceneLoad : MonoBehaviour
{
    private static void RestoreOriginalShaders() {
        Directory.Delete("Assets/Shader", recursive: true);
        var zip = ZipStorer.Open("Assets/Shader_Original.zip", FileAccess.Read);
        List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
        foreach (ZipStorer.ZipFileEntry entry in dir)
        {
                zip.ExtractFile(entry, "Assets/" + entry.FilenameInZip);
                break;
        }
        zip.Close();
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
