using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTestEntry : MonoBehaviour
{
    public Text m_name;
    public Text m_roundsName;
    public Text m_questionNames;
    public Text m_questionInRoundsName;

    private TestsList m_test;
    private TestCreatorPanel m_testCreatorPanel;

    public void SetTestCreatorPanel(TestCreatorPanel panel)
    {
        m_testCreatorPanel = panel;
    }

    public void SetTest(TestInfo test)
    {
        SetIfExists(m_name, test.m_name);
        SetIfExists(m_questionNames, test.m_totalCount.ToString());
        SetIfExists(m_roundsName, test.m_rounds.ToString());
        SetIfExists(m_questionInRoundsName, test.m_countInRound.ToString());
    }

    private void SetIfExists(Text txt, string value)
    {
        if (txt)
        {
            txt.text = value;
        }
    }

    public void OnDeleteTest()
    {
        m_testCreatorPanel.DeleteTest(this);
    }

    public void OnEditTest()
    {
        m_testCreatorPanel.EditTest(this);
    }
}
