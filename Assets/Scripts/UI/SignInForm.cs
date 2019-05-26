using PlayFab;
using PlayFab.ClientModels;
using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignInForm : MonoBehaviour
{
    public GameObject m_signUpForm;
    public InputField m_login;
    public InputField m_password;
    public Text m_message;
    public GameObject m_mainPanel;
    public GameObject m_registrationForm;

    public void OnGoToSingUp()
    {
        if (m_signUpForm)
        {
            OnResetMessage();
            m_signUpForm.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnSignIn()
    {
        Debug.Log("TrySingIn");
        var request = new LoginWithPlayFabRequest {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerStatistics = true },
            Username = m_login.text, Password = m_password.text };
        LobbyManager.m_playerName = m_login.text;
        PlayFabClientAPI.LoginWithPlayFab(request, OnSignInSuccess, OnSignInFailure);
    }

    private void OnSignInSuccess(LoginResult result)
    {
        GameManager.m_playfabID = result.PlayFabId;
        var stat = result.InfoResultPayload.PlayerStatistics.Find((StatisticValue sv) => { return sv.StatisticName == GameManager.PLAYFAB_TOTAL_POINTS; });
        GameManager.m_experience = stat.Value;
        m_mainPanel.gameObject.SetActive(true);
        m_mainPanel.gameObject.SetActive(false);
        m_mainPanel.gameObject.SetActive(true);
        m_registrationForm.SetActive(false);
    }

    private void OnSignInFailure(PlayFabError error)
    {
        OnShowMessage(error.GenerateErrorReport());
    }

    public void OnResetMessage()
    {
        if (m_message)
        {
            m_message.gameObject.SetActive(false);
        }
    }

    public void OnShowMessage(string message)
    {
        if (m_message)
        {
            m_message.gameObject.SetActive(true);
            m_message.text = message;
        }
    }
}
