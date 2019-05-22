using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//Purpose of that class is syncing data between server - client
public class PlayerSetup : NetworkBehaviour
{
    [Header("Network")]
    [Space]

    [SyncVar]
    public int m_PlayerNumber;

    [SyncVar]
    public string m_PlayerName;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer) //if not hosting, we had the tank to the gamemanger for easy access!
            GameManager.AddPlayer(gameObject, m_PlayerNumber, m_PlayerName);
    }

    public override void OnNetworkDestroy()
    {
        GameManager.s_Instance.RemovePlayer(gameObject);
    }
}
