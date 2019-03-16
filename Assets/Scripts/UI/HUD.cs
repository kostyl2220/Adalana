using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text m_score;
    public Text m_time;
    public Text m_name;
    public Text m_round;

    public void SetName(string name)
    {
        if (m_name)
        {
            m_name.text = name;
        }
    }

    public void SetTime(string time)
    {
        if (m_time)
        {
            m_time.text = time;
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
}
