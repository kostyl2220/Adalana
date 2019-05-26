using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreatorPanel : MonoBehaviour
{
    public EditTestEntry m_editTestEntry;
    public GameObject m_mainMenu;
    public EditTestPanel m_editTestPanel;
    public Transform m_testArea;

    private List<EditTestEntry> m_entries = new List<EditTestEntry>();

    private void OnEnable()
    {
        SetupTests();
    }

    void SetupTests()
    {
        foreach (var entry in m_entries)
        {
            Destroy(entry.gameObject);
        }

        m_entries.Clear();
        List<TestInfo> infos = TestReader.GetAllTestsInfo();
        foreach (var info in infos)
        {
            AddTest(info);
        }
    }

    public void AddTest(TestInfo testInfo)
    {
        EditTestEntry testEntry = Instantiate(m_editTestEntry, m_testArea);
        testEntry.SetTest(testInfo);
        testEntry.SetTestCreatorPanel(this);
        m_entries.Add(testEntry);
    }

    internal void DeleteTest(EditTestEntry entry)
    {
        var name = entry.m_name.text;

        TestReader.DeleteTest(name);
        m_entries.Remove(entry);
        Destroy(entry.gameObject);
    }

    internal void EditTest(EditTestEntry entry)
    {
        var name = entry.m_name.text;

        if (m_editTestPanel)
        {
            m_editTestPanel.SetTestName(name);
            m_editTestPanel.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnBack()
    {
        if (m_mainMenu)
        {
            m_mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnCreateNewTest()
    {
        if (m_editTestPanel)
        {
            m_editTestPanel.ResetTest();
            m_editTestPanel.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
