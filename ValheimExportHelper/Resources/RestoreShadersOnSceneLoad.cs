using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class RestoreShadersOnSceneLoad : MonoBehaviour
{
    
    [MenuItem("Valheim/Restore Shaders")]
    public static void RestoreOriginalShaders() {
        UnzipExtension.UnZipFile(Path.Combine("Assets", "Shader_Original.zip"));
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