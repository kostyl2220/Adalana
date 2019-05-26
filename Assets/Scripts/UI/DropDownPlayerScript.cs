using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.UI;

public class DropDownPlayerScript : MonoBehaviour
{
    private Dictionary<string, string> m_players = new Dictionary<string, string>();
    public Dropdown m_dropdown;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_dropdown)
        {
            m_dropdown = gameObject.GetComponent<Dropdown>();
        }
        if (m_dropdown)
        {
            m_dropdown.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }
    }

    public void OnEnable()
    {
        if (m_dropdown)
        {
            GetPlayers();
        }
    }

    private void GetPlayers()
    {
        var request = new GetLeaderboardRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints { ShowTags = true, ShowDisplayName = true }
            , StatisticName = GameManager.PLAYFAB_LAST_LOG_IN
        };

        PlayFabClientAPI.GetLeaderboard(request,
            OnPlayersReceived, Error);
    }

    private void Error(PlayFabError obj)
    {
        
    }

    private void OnPlayersReceived(GetLeaderboardResult obj)
    {
        m_dropdown.ClearOptions();
        m_players.Clear();

        List<string> result = new List<string>();
        foreach (var user in obj.Leaderboard)
        {
            if (user.PlayFabId == GameManager.m_playfabID)
            {
                continue;
            }

            if (user.Profile.Tags.Count == 0)
            {
                continue;
            }

            string tagIP = user.Profile.Tags[0].TagValue.Remove(0, 12);
            m_players[user.DisplayName] = tagIP;
            result.Add(user.DisplayName);
        }
        m_dropdown.AddOptions(result);
        OnValueChanged();
    }

    private void OnValueChanged()
    {
        if (m_dropdown.options.Count == 0)
        {
            return;
        }

        string curValue = m_dropdown.options[m_dropdown.value].text;
        LobbyManager.m_selectedEnemy = m_players[curValue];
    }

}
