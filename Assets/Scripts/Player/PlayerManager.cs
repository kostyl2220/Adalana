using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.
    
    [HideInInspector]
    public int m_PlayerNumber;                // This specifies which player this the manager for.
    [HideInInspector]
    public GameObject m_Instance;             // A reference to the instance of the tank when it is created.
    [HideInInspector]
    public int m_RightAnswers;                        // The number of wins this player has so far.
    [HideInInspector]
    public int m_Wins;                        // The number of wins this player has so far.
    [HideInInspector]
    public string m_PlayerName;                    // The player name set in the lobby    

    [HideInInspector]
    public bool m_loadReady = false;

    public int m_totalScore;
    public int m_lostedScore;

    public PlayerSetup m_Setup;
    public List<int> m_resultList = new List<int>();

    public void Setup()
    {
        // Get references to the components.
        m_Setup = m_Instance.GetComponent<PlayerSetup>();

        m_resultList.Clear();
        m_totalScore = 0;
        m_lostedScore = 0;
        m_RightAnswers = 0;
        m_Setup.m_PlayerName = m_PlayerName;
        m_Setup.m_PlayerNumber = m_PlayerNumber;
    }

    public string GetName()
    {
        return m_PlayerName;
    }

    public bool IsLocalPlayer()
    {
        return m_Setup.isLocalPlayer;
    }

    public void AddScore(int score)
    {
        m_totalScore += score;
        m_RightAnswers += score;
    }

    public void AddLostedScore(int score)
    {
        m_lostedScore += score;
    }

    public void RoundEnded()
    {
        m_resultList.Add(m_RightAnswers);
    }

    // Used at the start of each round to put the tank into it's default state.
    public void Reset()
    {
        m_RightAnswers = 0;
    }
}
