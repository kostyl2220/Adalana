using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestReader 
{
    public static string TEST_FOLDER = "Tests";
    public static string JSON_TYPE = ".json";

    public static List<TestInfo> GetAllTestsInfo()
    {
        List<TestInfo> list = new List<TestInfo>();

        string[] filePaths = Directory.GetFiles(GetTestFolderPath(), "*" + JSON_TYPE);
        foreach (var path in filePaths)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var testsList = GetTestList(fileName);
            if (testsList != null)
            {
                TestInfo info = new TestInfo { m_name = fileName, m_rounds = testsList.m_countOfRounds, m_countInRound = testsList.m_countQuestionsInRound, m_totalCount = testsList.m_tests.Count };
                list.Add(info);
            }
        }

        return list;
    }

    public static void DeleteTest(string testName)
    {
        string fullPath = GetFullPath(testName);

        if (!File.Exists(fullPath))
        {
            return;
        }

        File.Delete(fullPath);
    }

    public static TestsList GetTestList(string fileName)
    {
        string fullPath = GetFullPath(fileName);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return JsonUtility.FromJson<TestsList>(File.ReadAllText(fullPath));
    }

    public static void SaveTestList(TestsList list)
    {
        SaveTestList(list.m_name, list);
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
        return Path.Combine(GetTestFolderPath(), fileName) + JSON_TYPE;
    }

    public static string GetTestFolderPath()
    {
        return Path.Combine(Application.dataPath, TEST_FOLDER);
    }
}
