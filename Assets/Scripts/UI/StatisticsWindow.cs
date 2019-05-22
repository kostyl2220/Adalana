using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsWindow : MonoBehaviour
{
    public static Dictionary<string, string> statistics =
    new Dictionary<string, string> {
       { "WinCount", "Wins" },
       { "LoseCount", "Loses" },
       { "DrawCount", "Draws" },
       { "TotalPoints", "Right answers"},
       { "LostPoints", "Wrong answers"},
    };

    public static string[] sumValues = { "WinCount", "LoseCount", "DrawCount" };

    public static string TOTAL_GAMES = "Played games";

    public GameObject m_mainMenu;
    public GameObject m_loadingText;
    public StatisticsField m_field;
    public Transform m_statArea;

    private List<StatisticsField> m_stats = new List<StatisticsField>();
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
    }

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

    public void OnResetPlayerStatistics()
    {
        var statisticList = new List<StatisticUpdate>();
        foreach (var value in statistics.Keys)
        {
            statisticList.Add(new StatisticUpdate { StatisticName = value, Value = 0, Version = 0 });
        }

        var request = new UpdatePlayerStatisticsRequest { Statistics = statisticList };
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => { ReadStatistics(); },
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
        foreach (var eachStat in result.Statistics)
        {
            AddStatisticsField(statistics[eachStat.StatisticName], eachStat.Value);
        }
    }

    private void AddStatisticsField(string name, int value)
    {
        StatisticsField field = Instantiate(m_field, m_statArea);
        field.SetName(name);
        field.SetValue(value.ToString());
        m_stats.Add(field);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
