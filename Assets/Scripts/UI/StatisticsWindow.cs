using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsWindow : MonoBehaviour
{
    public static Dictionary<string, string> statistics =
    new Dictionary<string, string> {
       { "1WinCount", "Wins" },
       { "2LoseCount", "Loses" },
       { "3DrawCount", "Draws" },
       { "4TotalPoints", "Right answers"},
       { "5LostPoints", "Wrong answers"},
    };

    public static string[] sumValues = { "1WinCount", "2LoseCount", "3DrawCount" };

    public static string TOTAL_GAMES = "Played games";

    public GameObject m_mainMenu;
    public GameObject m_loadingText;
    public GameObject m_leaderboard;
    public StatisticsField m_field;
    public Transform m_statArea;

    private List<StatisticsField> m_stats = new List<StatisticsField>();

    private void OnEnable()
    {
        ReadStatistics();
    }

    public void OnBack()
    {
        if (m_mainMenu)
        {
            m_mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnLeaderboard()
    {
        if (m_leaderboard)
        {
            m_leaderboard.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public static void OnSetupPlayerStatistics()
    {
        var statisticList = new List<StatisticUpdate>();
        foreach (var value in statistics.Keys)
        {
            statisticList.Add(new StatisticUpdate { StatisticName = value, Value = 0, Version = 0 });
        }

        var request = new UpdatePlayerStatisticsRequest { Statistics = statisticList };
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => { },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    private void ReadStatistics()
    {
        foreach (var stat in m_stats)
        {
            Destroy(stat.gameObject);
        }
        m_stats.Clear();
        m_loadingText.SetActive(true);

        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    public void ResetStatistics()
    {
        var request = new PlayFab.AdminModels.ResetUserStatisticsRequest
        {
            PlayFabId = GameManager.m_playfabID
        };

        PlayFabAdminAPI.ResetUserStatistics(request,
            result => { ReadStatistics(); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        m_loadingText.SetActive(false);
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);

        int totalGameCount = 0;
        foreach (var eachStat in result.Statistics)
        {
            foreach (var value in sumValues)
            {
                if (eachStat.StatisticName == value)
                {
                    totalGameCount += eachStat.Value;
                }
            }
        }

        AddStatisticsField(TOTAL_GAMES, totalGameCount);
        result.Statistics.Sort((StatisticValue val1, StatisticValue val2) => { return val1.StatisticName.CompareTo(val2.StatisticName); });
        foreach (var eachStat in result.Statistics)
        {
            if (statistics.ContainsKey(eachStat.StatisticName))
            {
                AddStatisticsField(statistics[eachStat.StatisticName], eachStat.Value);
            }
        }
    }

    private void AddStatisticsField(string name, int value)
    {
        StatisticsField field = Instantiate(m_field, m_statArea);
        field.SetName(name);
        field.SetValue(value.ToString());
        m_stats.Add(field);
    }
}
