/**
 * IMPORTANT: Remove this when it gets added to AssetRipper.
 * Sourced from AssetRipper Discord, © trouger
*/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class AvoidSavingYamlShaders : UnityEditor.AssetModificationProcessor
{
    private static List<string> s_PathList = new List<string>();
    
    private static string[] OnWillSaveAssets(string[] paths)
    {
        s_PathList.Clear();
        
        foreach (string path in paths)
        {
            if (!path.EndsWith(".asset") || !(AssetDatabase.LoadMainAssetAtPath(path) is Shader))
            {
                s_PathList.Add(path);
            }
        }
        return s_PathList.ToArray();
    }
}
