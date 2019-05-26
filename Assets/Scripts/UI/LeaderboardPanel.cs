using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
    public GameObject m_prevPage;
    public GameObject m_loadingText;
    public StatisticsField m_field;
    public Transform m_statArea;

    private List<StatisticsField> m_stats = new List<StatisticsField>();
    private string m_currentLeaderboard = GameManager.PLAYFAB_TOTAL_POINTS;

    public void OnEnable()
    {
        ReadLeaderboard();
    }

    public void OnBack()
    {
        if (m_prevPage)
        {
            m_prevPage.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void TrySetLeaderboard(string newValue)
    {
        if (m_currentLeaderboard == newValue)
        {
            return;
        }

        m_currentLeaderboard = newValue;
        ReadLeaderboard();
    }

    public void OnScore()
    {
        TrySetLeaderboard(GameManager.PLAYFAB_TOTAL_POINTS);
    }

    public void OnWin()
    {
        TrySetLeaderboard(GameManager.PLAYFAB_WIN_COUNT);
    }

    private void ReadLeaderboard()
    {
        foreach (var stat in m_stats)
        {
            Destroy(stat.gameObject);
        }
        m_stats.Clear();
        m_loadingText.SetActive(true);

        var request = new GetLeaderboardRequest
        {
            StatisticName = m_currentLeaderboard,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request,
            OnLeaderboardReceived, Error);
    }

    private void Error(PlayFabError obj)
    {
        throw new NotImplementedException();
    }

    private void OnLeaderboardReceived(GetLeaderboardResult obj)
    {
        m_loadingText.SetActive(false);

        foreach (var user in obj.Leaderboard)
        {
            AddLeaderboardField(user.Position, user.DisplayName, user.StatValue);
        }
    }

    private void AddLeaderboardField(int number, string name, int value)
    {
        StatisticsField field = Instantiate(m_field, m_statArea);
        field.SetNumber(number);
        field.SetName(name);
        field.SetValue(value.ToString());
        m_stats.Add(field);
    }
}
