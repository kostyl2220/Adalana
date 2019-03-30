using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class fasade
public class HUD : MonoBehaviour
{
    [Serializable]
    public struct PlayerData
    {
        public Text Name;
        public GameObject PartialAnswer;
        public StarHolder StarHolder;
    }

    public Text m_score;
    public Text m_time;
    public Text m_round;
    public Text m_question;
    public GameObject m_questionSection;
    public Text m_questionText;
    public PlayerData[] playerData = new PlayerData[2];
    public FadeOutFadeIn m_screenMessage;

    private int m_totalRounds;

    public void ShowScreenMessage(string message)
    {
        if (m_screenMessage)
        {
            m_screenMessage.SetText(message);
        }
    }

    public void ShowPlayerInfo(int playerId, bool show)
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
            m_score.text = player2.ToString() + ":" + player1.ToString();
        }
    }

    public void SetRound(int round)
    {
        if (m_round)
        {
            m_round.text = "ROUND " + round.ToString() + "/" + m_totalRounds;
        }
    }

    public void SetTotalRounds(int rounds)
    {
        m_totalRounds = rounds;
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
        if (playerId == GameManager.INVALID_PLAYER_ID)
        {
            foreach (var data in playerData)
            {
                data.PartialAnswer.SetActive(false);
            }
           
        }
        else if (playerData[playerId].PartialAnswer)
        {
            playerData[playerId].PartialAnswer.SetActive(true);
        }
    }

    public void AddStar(int playerId)
    {
        if (playerId == GameManager.INVALID_PLAYER_ID)
        {
            return;
        }

        if (playerData[playerId].StarHolder)
        {
            playerData[playerId].StarHolder.AddStar();
        }
    }
}
