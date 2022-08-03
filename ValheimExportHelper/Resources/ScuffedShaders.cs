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
        Directory.Delete("Assets/Shader", recursive: true);
        ZipFile.ExtractToDirectory("Assets/Shader_Original.zip", "Assets");
    }

    static ScuffedShaders() {
        RestoreOriginalShaders();

        EditorSceneManager.sceneOpening += (path, mode) => {
            RestoreOriginalShaders();
        };
    }
}
