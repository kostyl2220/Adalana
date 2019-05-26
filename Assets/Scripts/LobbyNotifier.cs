using PlayFab;
using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class LobbyNotifier : MonoBehaviour
{
    private string m_IP;

    private void OnEnable()
    {
        if (LobbyManager.m_playSolo)
        {
            return;
        }

        GameManager.UpdatePlayerStatistics(GameManager.PLAYFAB_LAST_LOG_IN, (int)System.DateTime.Now.Ticks);
        m_IP = LocalIPAddress();
        var request = new PlayFab.AdminModels.AddPlayerTagRequest { TagName = m_IP, PlayFabId = GameManager.m_playfabID };
        PlayFabAdminAPI.AddPlayerTag(request,
            result => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    private void OnDisable()
    {
        if (LobbyManager.m_playSolo)
        {
            return;
        }

        GameManager.UpdatePlayerStatistics(GameManager.PLAYFAB_LAST_LOG_IN, 0);
        var request = new PlayFab.AdminModels.RemovePlayerTagRequest { TagName = m_IP, PlayFabId = GameManager.m_playfabID };
        PlayFabAdminAPI.RemovePlayerTag(request,
            result => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    public static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
