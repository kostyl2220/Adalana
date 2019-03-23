using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Serializable]
    public struct PlayerData
    {
        public Text Name;
        public GameObject PartialAnswer;
    }

    public Text m_score;
    public Text m_time;
    public Text m_round;
    public Text m_question;
    public GameObject m_questionSection;
    public Text m_questionText;
    public PlayerData[] playerData = new PlayerData[2];

    public void ShowPlayerName(int playerId, bool show)
    {
        if (playerData[playerId].Name)
        {
            playerData[playerId].Name.transform.parent.gameObject.SetActive(show);
        }
    }

    public void SetPlayerName(int playerId, string name)
    {
        if (playerData[playerId].Name)
        {
            playerData[playerId].Name.text = name;
        }
    }

    public void SetTime(float time)
    {
        if (m_time)
        {
            m_time.text = string.Format("{0, 0:D2}:{1, 0:D2}", (int)(time / 60), (int)time % 60);
        }
    }

    public void SetScore(int player1, int player2)
    {
        if (m_score)
        {
            m_score.text = player1.ToString() + ":" + player2.ToString();
        }
    }

    public void SetRound(int round)
    {
        if (m_round)
        {
            m_round.text = "ROUND " + round.ToString();
        }
    }

    public void HideQuestion()
    {
        m_questionSection.SetActive(false);
    }

    public void SetQuestion(int question)
    {
        if (m_questionSection)
        {
            m_questionSection.SetActive(true);
        }

        if (m_question)
        {
            m_question.text = "Question " + question;
        }
    }

    public void SetQuestion(string name)
    {
        if (m_questionText)
        {
            m_questionText.text = name;
        }
    }

    public void SetPartialAnswer(int playerId)
    {
        if (playerId < 0 || playerId > 1)
        {
            foreach (var data in playerData)
            {
                data.PartialAnswer.SetActive(false);
            }
            return;
        }

        if (playerData[playerId].PartialAnswer)
        {
            playerData[playerId].PartialAnswer.SetActive(true);
        }
    }
}
