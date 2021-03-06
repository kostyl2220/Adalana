﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;
using Prototype.NetworkLobby;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;

public class SignUpForm : MonoBehaviour
{
    public GameObject m_signInForm;
    public InputField m_login;
    public InputField m_password;
    public InputField m_passConfirmation;
    public Text m_message;
    public GameObject m_mainPanel;
    public GameObject m_registrationForm;

    public void OnSignUp()
    {
        Debug.Log("TrySingUp");
        if (m_password.text != m_passConfirmation.text)
        {
            OnShowMessage("Type same passwords");
            return;
        }
        var request = new RegisterPlayFabUserRequest { DisplayName = m_login.text, Username = m_login.text, Password = m_password.text, RequireBothUsernameAndEmail = false };
        LobbyManager.m_playerName = m_login.text;
        PlayFabClientAPI.RegisterPlayFabUser(request, OnSignUpSuccess, OnSignUPFailure);
    }

    private void OnSignUpSuccess(RegisterPlayFabUserResult obj)
    {
        StatisticsWindow.OnSetupPlayerStatistics();
        GameManager.m_playfabID = obj.PlayFabId;
        m_mainPanel.gameObject.SetActive(true);
        m_mainPanel.gameObject.SetActive(false);
        m_mainPanel.gameObject.SetActive(true);
        m_registrationForm.SetActive(false);
    }

    private void OnSignUPFailure(PlayFabError error)
    {
        OnShowMessage(error.GenerateErrorReport());
    }

    public void OnBack()
    {
        if (m_signInForm)
        {
            OnResetMessage();
            m_signInForm.SetActive(true);
            gameObject.SetActive(false);
        }
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
