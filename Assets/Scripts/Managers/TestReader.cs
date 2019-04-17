using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestReader 
{
    static string TEST_FOLDER = "Tests";
    static string JSON_TYPE = ".json";

    public static TestsList GetTestList(string fileName)
    {
        string fullPath = GetFullPath(fileName);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return JsonUtility.FromJson<TestsList>(File.ReadAllText(fullPath));
    }

    public static void SaveTestList(string fileName, TestsList list)
    {
        string fullPath = GetFullPath(fileName);

        if (!File.Exists(fullPath))
        {
            File.Create(fullPath);
        }

        File.WriteAllText(fullPath, JsonUtility.ToJson(list));
    }

    private static string GetFullPath(string fileName)
    {
        return Path.Combine(Path.Combine(Application.dataPath, TEST_FOLDER), fileName) + JSON_TYPE;
    }
}
