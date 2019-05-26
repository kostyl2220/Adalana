using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;
        public GameObject m_prevPanel;
        public GameObject m_progressBar;

        public RectTransform lobbyPanel;

        public Dropdown ipInput;

        private bool m_gameInited = false;

        public void OnEnable()
        {
            LobbyManager.m_playSolo = false;
            lobbyManager.topPanel.ToggleVisibility(true);
            if (m_gameInited)
            {
                m_gameInited = false;
                NetworkManager.singleton.StopClient();
                NetworkManager.singleton.StopHost();
                NetworkManager.singleton.StopMatchMaker();
            }
        }

        public void OnClickHost()
        {
            m_gameInited = true;
            lobbyManager.StartHost();
            DisableMenuScene();
        }

        private void DisableMenuScene()
        {
            gameObject.SetActive(false);
            if (m_progressBar)
            {
                m_progressBar.SetActive(false);
            }
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = LobbyManager.m_selectedEnemy;
            lobbyManager.StartClient();

            DisableMenuScene();
        }

        public void OnPlaySolo()
        {
            m_gameInited = true;
            LobbyManager.m_playSolo = true;
            lobbyManager.StartHost();
            DisableMenuScene();
        }

        public void OnBack()
        {
            if (m_prevPanel)
            {
                m_prevPanel.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
