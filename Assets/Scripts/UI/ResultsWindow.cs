using Prototype.NetworkLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsWindow : MonoBehaviour
{
    public Text m_firstPlayer;
    public Text m_seondPlayer;
    public Text m_results;
    public Text m_versus;
    public Text m_versus2;
    public StatisticsField m_instance;
    public Transform m_resultArea;

    private List<StatisticsField> m_resultList = new List<StatisticsField>();

    public void ToMainMenu()
    {
        LobbyManager.s_Singleton.SendReturnToLobby();
    }

    public void SetResutInfo(int[] results0, int[] results1, bool multiplayer, string fName, string sName)
    {
        ResetResult();
        int[] res = new int[2];
        for (int i = 0; i < results0.Length; ++i)
        {      
            if (multiplayer)
            {

                if (results0[i] != results1[i])
                {
                    int id = results0[i] > results1[i] ? 0 : 1;
                    res[id]++;
                }

                AddResult(results1[i], results0[i], i);
            }
            else
            {
                AddResult(-1, results0[i], i);
            }
        }
        SetupResult(res);
        m_firstPlayer.text = fName;
        m_seondPlayer.text = sName;
        EnableCoop(multiplayer);
    }

    private void SetupResult(int[] res)
    {
        if (res.Length == 1)
        {
            m_results.text = res[0].ToString();
        }
        else
        {
            m_results.text = res[1].ToString() + "/" + res[0].ToString();
        }
    }

    private void EnableCoop(bool coop)
    {
        m_seondPlayer.gameObject.SetActive(coop);
        m_versus.gameObject.SetActive(coop);
        m_versus2.gameObject.SetActive(coop);
    }

    private void ResetResult()
    {
        foreach (var res in m_resultList)
        {
            Destroy(res.gameObject);
        }

        m_resultList.Clear();
    }

    private void AddResult(int left, int right, int roundNum)
    {
        var result = Instantiate(m_instance, m_resultArea);
        result.SetValue(right.ToString());
        result.SetName(left.ToString());
        result.SetNumber(roundNum + 1);
        result.m_statName.gameObject.SetActive(left != -1);
        m_resultList.Add(result);
    }
}
