using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        var request = new LoginWithPlayFabRequest { Username = m_login.text, Password = m_password.text };
        PlayFabClientAPI.LoginWithPlayFab(request, OnSignInSuccess, OnSignInFailure);
    }

    private void OnSignInSuccess(LoginResult result)
    {
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
