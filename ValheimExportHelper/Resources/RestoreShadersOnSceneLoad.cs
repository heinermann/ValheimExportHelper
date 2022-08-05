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
    public static void RestoreOriginalShaders() {
        if (Directory.Exists("Assets/Shader"))
        {
            if(Directory.EnumerateFiles("Assets/Shader").ToList().Count >=1)
            {
                Directory.Delete("Assets/Shader", recursive: true);
            }
        }
        
        var zip = ZipStorer.Open("Assets"+ Path.DirectorySeparatorChar +"Shader_Original.zip", FileAccess.Read);
        List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
        foreach (ZipStorer.ZipFileEntry entry in dir)
        {
                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strWorkPath = Path.GetDirectoryName(strExeFilePath);
                char[] trimcharacters =
                {
                     'S', 'c', 'r', 'i', 'p', 't', 'A', 's', 's', 'e', 'm', 'b', 'l', 'i', 'e', 's'
                };
                char[] trimchar2 =
                {
                    'L', 'i', 'b', 'r', 'a', 'r', 'y'
                };
                
                string finalPath = strWorkPath.TrimEnd(trimcharacters);
                string correctedPath = finalPath.TrimEnd(Path.DirectorySeparatorChar);
                string correctedPath1 = correctedPath.TrimEnd(trimchar2);
                string forrealPath = correctedPath1  + "Assets" + Path.DirectorySeparatorChar;
                zip.ExtractFile(entry, forrealPath + entry.FilenameInZip);
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
