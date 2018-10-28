using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetUtils
{
    public static List<T> GetAssetList<T>(string path) where T : class
    {
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);

        return fileEntries.Select(fileName =>
        {
            string temp = fileName.Replace("\\", "/");
            int index = temp.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += temp.Substring(index);

            return AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
        })
            //Filtering null values, the Where statement does not work for all types T
            .OfType<T>()    //.Where(asset => asset != null)
            .ToList();
    }
}
