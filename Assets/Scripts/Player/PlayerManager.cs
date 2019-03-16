using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.

    public Color m_PlayerColor;               // This is the color this tank will be tinted.
    [HideInInspector]
    public int m_PlayerNumber;                // This specifies which player this the manager for.
    [HideInInspector]
    public GameObject m_Instance;             // A reference to the instance of the tank when it is created.
    [HideInInspector]
    public int m_Wins;                        // The number of wins this player has so far.
    [HideInInspector]
    public string m_PlayerName;                    // The player name set in the lobby
    [HideInInspector]
    public int m_LocalPlayerID;                    // The player localID (if there is more than 1 player on the same machine)

    public PlayerSetup m_Setup;

    public void Setup()
    {
        // Get references to the components.
        m_Setup = m_Instance.GetComponent<PlayerSetup>();

        m_Setup.m_Color = m_PlayerColor;
        m_Setup.m_PlayerName = m_PlayerName;
        m_Setup.m_PlayerNumber = m_PlayerNumber;
        m_Setup.m_LocalID = m_LocalPlayerID;
    }

    public string GetName()
    {
        return m_Setup.m_PlayerName;
    }

    public void SetLeader(bool leader)
    {
        m_Setup.SetLeader(leader);
    }

    public bool IsReady()
    {
        return m_Setup.m_IsReady;
    }

    // Used at the start of each round to put the tank into it's default state.
    public void Reset()
    {

    }
}
