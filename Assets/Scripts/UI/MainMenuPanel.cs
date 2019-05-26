using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : MonoBehaviour
{
    public GameObject m_statisticsPanel;
    public GameObject m_playPanel;
    public RadialProgress m_progressBar;
    public GameObject m_testCreator;

    private void OnEnable()
    {
        if (m_progressBar)
        {
            m_progressBar.Activate();
        }
    }

    public void OnStaticstics()
    {
        if (m_statisticsPanel)
        {
            m_statisticsPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnTestCreator()
    {
        if (m_testCreator)
        {
            m_testCreator.SetActive(true);
            if (m_progressBar)
            {
                m_progressBar.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    public void OnPlay()
    {
        if (m_playPanel)
        {
            m_playPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
