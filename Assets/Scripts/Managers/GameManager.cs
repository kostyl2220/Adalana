using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using Prototype.NetworkLobby;


public class GameManager : NetworkBehaviour
{
    static string PARTIAL_ANSWER_MESSAGE = "PARTIALLY ANSERED";
    static string WIN_MESSAGE = "YOU WIN";
    static string LOSE_MESSAGE = "YOU LOSE";
    static string DRAW_MESSAGE = "DRAW";

    static public int INVALID_PLAYER_ID = -1;
    static private int INVALID_PARTIAL_SELECTED = -1;
    static public GameManager s_Instance;

    //this is static so tank can be added even withotu the scene loaded (i.e. from lobby)
    static public List<PlayerManager> m_Players = new List<PlayerManager>();             // A collection of managers for enabling and disabling different aspects of the tanks.

    public int m_NumRoundsToWin = 5;          // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;             // The delay between the end of RoundPlaying and RoundEnding phases.

    [HideInInspector]
    [SyncVar]
    public bool m_GameIsFinished = false;

    private bool m_answerSelected = false;
    private int m_partialAnsweredPlayerId = INVALID_PARTIAL_SELECTED;

    //Various UI references to hide the screen between rounds.
    [Space]
    [Header("UI")]
    public CanvasGroup m_FadingScreen;
    public CanvasGroup m_EndRoundScreen;
    public HUD m_hud;

    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private int m_RoundWinnerId;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private int m_GameWinnerId;           // Reference to the winner of the game.  Used to make an announcement of who won.
    private int m_localPlayerID;
    private List<TestModule> m_testModules;
    private TestModule m_currentTest;
    private TestsList m_currentTestList;

    void Awake()
    {
        s_Instance = this;
    }

    private void Hack_SetupTestList()
    {
        m_currentTestList = new TestsList();

        List<Variant> vars1 = new List<Variant>
            {
             new Variant("Hello", 1)
            , new Variant("World", 2)
            , new Variant("How are", 3)
            , new Variant("you?", 4)
        };

        List<int> answers1 = new List<int>{ 1, 2, 3, 4 };

        Test test1 = new Test
        {
            m_type = TestTypes.ARRANGE,
            m_question = "Combine first program correctly",
            m_answerTime = 15.0f,
            m_variants = vars1,
            m_answers = answers1
        };

        List<Variant> vars2 = new List<Variant>
            {
             new Variant("Lol", 1)
            , new Variant("Kek", 2)
            , new Variant("Chebu", 3)
            , new Variant("Rek", 4)
        };

        List<int> answers2 = new List<int> { 1, 2, 3, 4 };

        Test test2 = new Test
        {
            m_type = TestTypes.ARRANGE,
            m_question = "Piu",
            m_answerTime = 12.0f,
            m_variants = vars2,
            m_answers = answers2
        };

        m_currentTestList.m_countOfRounds = 2;
        m_currentTestList.m_countQuestionsInRound = 5;
        for (int i = 0; i < 5; ++i)
        {
            m_currentTestList.m_tests.Add(test1);
            m_currentTestList.m_tests.Add(test2);
        }

        m_currentTestList.InitTests();
    }

    [ClientRpc]
    public void RpcSetupGame()
    {
        SetupLocalPlayerID();
        SetupPlayerNames();
        SetupModules();
        Debug.Log("Hello");
        //TODO only on server
        Hack_SetupTestList();
        m_hud.SetTotalRounds(m_currentTestList.m_countOfRounds);
        CmdPlayerReady(m_localPlayerID);
    }

    private void SetupLocalPlayerID()
    {
        for (int i = 0; i < m_Players.Count; ++i)
        {
            if (m_Players[i].m_Setup.isLocalPlayer)
            {
                m_localPlayerID = i;
                return;
            }
        }
    }

    private void SetupModules()
    {
        m_testModules = new List<TestModule>();
        foreach (Transform module in transform)
        {
            ArrangeTestModule atm = module.GetComponent<ArrangeTestModule>();
            m_testModules.Add(atm);
        }
    }

    [ServerCallback]
    private void Start()
    {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine(GameLoop());
    }

    /// <summary>
    /// Add a tank from the lobby hook
    /// </summary>
    /// <param name="tank">The actual GameObject instantiated by the lobby, which is a NetworkBehaviour</param>
    /// <param name="playerNum">The number of the player (based on their slot position in the lobby)</param>
    /// <param name="c">The color of the player, choosen in the lobby</param>
    /// <param name="name">The name of the Player, choosen in the lobby</param>
    /// <param name="localID">The localID. e.g. if 2 player are on the same machine this will be 1 & 2</param>
    static public void AddTank(GameObject tank, int playerNum, Color c, string name, int localID)
    {
        PlayerManager tmp = new PlayerManager();
        tmp.m_Instance = tank;
        tmp.m_PlayerNumber = playerNum;
        tmp.m_PlayerColor = c;
        tmp.m_PlayerName = name;
        tmp.m_LocalPlayerID = localID;
        tmp.Setup();
        m_Players.Add(tmp);
    }

    public void RemoveTank(GameObject tank)
    {
        PlayerManager toRemove = null;
        foreach (var tmp in m_Players)
        {
            if (tmp.m_Instance == tank)
            {
                toRemove = tmp;
                break;
            }
        }

        if (toRemove != null)
            m_Players.Remove(toRemove);
    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop()
    {
        while (m_Players.Count < 1)
            yield return null;

        RpcSetupGame();

        while (!AllPlayersReady())
            yield return null;

        while (m_currentTestList.IsRoundAvaliable())
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundStarting());
            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());
            // Once execution has returned here, run the 'RoundEnding' coroutine.
            yield return StartCoroutine(RoundEnding());
        }

        m_GameWinnerId = GetGameWinnerId();
        RpcEndGame(m_GameWinnerId);

        // This code is not run until 'RoundEnding' has finished.  At which point, check if there is a winner of the game.
        if (m_GameWinnerId != INVALID_PLAYER_ID)
        {// If there is a game winner, wait for certain amount or all player confirmed to start a game again
            m_GameIsFinished = true;
            float leftWaitTime = 15.0f;
            bool allAreReady = false;
            int flooredWaitTime = 15;

            while (leftWaitTime > 0.0f && !allAreReady)
            {
                yield return null;

                allAreReady = true;
                foreach (var tmp in m_Players)
                {
                    allAreReady &= tmp.IsReady();
                }

                leftWaitTime -= Time.deltaTime;

                int newFlooredWaitTime = Mathf.FloorToInt(leftWaitTime);

                if (newFlooredWaitTime != flooredWaitTime)
                {
                    flooredWaitTime = newFlooredWaitTime;
                    string message = EndMessage(flooredWaitTime);
                }
            }

            LobbyManager.s_Singleton.ServerReturnToLobby();
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    private bool AllPlayersReady()
    {
        foreach(var player in m_Players)
        {
            if (!player.m_loadReady)
            {
                return false;
            }
        }
        return true;
    }

    [Command]
    public void CmdPlayerReady(int playerID)
    {
        m_Players[playerID].m_loadReady = true;
    }

    private IEnumerator RoundStarting()
    {
        ResetAll();
        //we notify all clients that the round is starting
        RpcRoundStarting();

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    [ClientRpc]
    void RpcRoundStarting()
    {
        // Increment the round number and display text showing the players what round it is.
        m_hud.SetRound(m_currentTestList.GetCurrentRound());
    }

    private IEnumerator RoundPlaying()
    {     
        while (m_currentTestList.HasValidQuestionForRound())
        {
            RpcTestPlaying();

            m_answerSelected = false;
            float remainingTime = m_currentTestList.GetCurrentTest().m_answerTime;
            // While there is not one tank left...
            while (remainingTime > 0 && !m_answerSelected)
            {
                RpcUpdateTime(remainingTime);
                remainingTime -= Time.deltaTime;
                // ... return on the next frame.
                yield return null;
            }
            RpcTestAnswered();
            m_partialAnsweredPlayerId = INVALID_PARTIAL_SELECTED;
            RpcSetPartialAnswer(m_partialAnsweredPlayerId);           
            yield return null;
        }
    }

    [ClientRpc]
    void RpcTestPlaying()
    {
        m_answerSelected = false;
        Test currentTest = m_currentTestList.GetCurrentTest();
        
        m_currentTest = m_testModules[(int)currentTest.m_type];
        m_currentTest.ActivateScene(true);
        m_currentTest.SetupTest(currentTest);

        UpdateQuestion(currentTest.m_question);
        UpdateQuestion(m_currentTestList.GetCurrentQuestionNumber());
    }

    [ClientRpc]
    void RpcTestAnswered()
    {
        m_currentTestList.SetNextTest();
        m_currentTest.ActivateScene(false);
    }

    private IEnumerator RoundEnding()
    {
        // See if there is a winner now the round is over.
        m_RoundWinnerId = GetRoundWinnerId();
        // If there is a winner, increment their score.
        if (m_RoundWinnerId != INVALID_PLAYER_ID)
        {
            m_Players[m_RoundWinnerId].m_Wins++;
        }

        RpcRoundEnding(m_RoundWinnerId);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    [ClientRpc]
    private void RpcRoundEnding(int roundWinnerId)
    {
        if (m_Players.Count > 1)
        {
            m_hud.AddStar(roundWinnerId);
        }
        m_currentTestList.SetTestsForNextRound();
    }

    // This is used to check if there is one or fewer tanks remaining and thus the round should end.
    private bool OneTankLeft()
    {
        // Start the count of tanks left at zero.
        int numTanksLeft = 0;

        // If there are one or fewer tanks remaining return true, otherwise return false.
        return numTanksLeft <= 1;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private int GetRoundWinnerId()
    {
        int maxScoredPlayer = 0;
        bool uniqueScore = true;

        // Go through all the tanks...
        for (int i = 1; i < m_Players.Count; i++)
        {
            if (m_Players[i].m_RightAnswers > m_Players[maxScoredPlayer].m_RightAnswers)
            {
                uniqueScore = true;
                maxScoredPlayer = i;
            }
            else if (m_Players[i].m_RightAnswers == m_Players[maxScoredPlayer].m_RightAnswers)
            {
                uniqueScore = false;
            }
        }

        // If no tanks have enough rounds to win, return null.
        return uniqueScore ? maxScoredPlayer : INVALID_PLAYER_ID;
    }

    [ClientRpc]
    private void RpcEndGame(int winnerId)
    {
        m_hud.HideQuestion();

        if (winnerId == INVALID_PLAYER_ID)
        {
            ShowScreenMessage(DRAW_MESSAGE);
        }
        else
        {
            ShowScreenMessage(winnerId == m_localPlayerID ? WIN_MESSAGE : LOSE_MESSAGE);
        }
    }

    // This function is to find out if there is a winner of the game.
    private int GetGameWinnerId()
    {
        int maxScoredPlayer = 0;
        bool uniqueScore = true;

        // Go through all the tanks...
        for (int i = 1; i < m_Players.Count; i++)
        {
            if (m_Players[i].m_Wins > m_Players[maxScoredPlayer].m_Wins)
            {
                uniqueScore = true;
                maxScoredPlayer = i;
            }
            else if (m_Players[i].m_Wins == m_Players[maxScoredPlayer].m_Wins)
            {
                uniqueScore = false;
            }
        }

        // If no tanks have enough rounds to win, return null.
        return uniqueScore ? maxScoredPlayer : INVALID_PLAYER_ID;
    }


    // Returns a string of each player's score in their tank's color.
    private string EndMessage(int waitTime)
    {
        // By default, there is no winner of the round so it's a draw.
        string message = "DRAW!";


        // If there is a game winner set the message to say which player has won the game.
        if (m_GameWinnerId != INVALID_PLAYER_ID)
        {
            PlayerManager gameWinner = m_Players[m_GameWinnerId];
            message = "<color=#" + ColorUtility.ToHtmlStringRGB(gameWinner.m_PlayerColor) + ">" + gameWinner.m_PlayerName + "</color> WINS THE GAME!";
        }
        // If there is a winner, change the message to display 'PLAYER #' in their color and a winning message.
        else if (m_RoundWinnerId != INVALID_PLAYER_ID)
        {
            PlayerManager roundWinner = m_Players[m_GameWinnerId];
            message = "<color=#" + ColorUtility.ToHtmlStringRGB(roundWinner.m_PlayerColor) + ">" + roundWinner.m_PlayerName + "</color> WINS THE ROUND!";
        }

        // After either the message of a draw or a winner, add some space before the leader board.
        message += "\n\n";

        // Go through all the tanks and display their scores with their 'PLAYER #' in their color.
        for (int i = 0; i < m_Players.Count; i++)
        {
            message += "<color=#" + ColorUtility.ToHtmlStringRGB(m_Players[i].m_PlayerColor) + ">" + m_Players[i].m_PlayerName + "</color>: " + m_Players[i].m_Wins + " WINS "
                + (m_Players[i].IsReady() ? "<size=15>READY</size>" : "") + " \n";
        }

        if (m_GameWinnerId != INVALID_PLAYER_ID)
            message += "\n\n<size=20 > Return to lobby in " + waitTime + "\nPress Fire to get ready</size>";

        return message;
    }

    public void CheckAnswers()
    {
        CmdCheckAnswers(m_localPlayerID);
    }

    [Command]
    public void CmdCheckAnswers(int playerID)
    {
        if (playerID == m_partialAnsweredPlayerId)
        {
            return;
        }

        PlayerManager player = m_Players[playerID];
        Debug.Log("Answered");

        int score = m_currentTest.GetScore(m_currentTestList.GetCurrentTest().m_answers);
        player.m_RightAnswers += score;
        m_answerSelected = true;
        ServerUpdateScore();

        if (m_Players.Count < 2)
        {
            return;
        }

        //partialAnswer
        if (score != m_currentTestList.GetCurrentTest().m_answers.Count)
        {
            if (m_partialAnsweredPlayerId == INVALID_PARTIAL_SELECTED)
            {
                m_answerSelected = false;
            }
            m_partialAnsweredPlayerId = playerID;
            RpcSetPartialAnswer(playerID);
            SetPlayerMessageLocal(playerID, PARTIAL_ANSWER_MESSAGE);
        }
    }

    private void ServerUpdateScore()
    {
        RpcUpdateScore(m_Players[0].m_RightAnswers, m_Players.Count == 2 ? m_Players[1].m_RightAnswers : 0);
    }

    //on server
    public void SetPlayerMessageLocal(int playerID, string message)
    {
        if (m_localPlayerID == playerID)
        {
            ShowScreenMessage(message);
        }
        else
        {
            ClientShowScreenMessage(message);
        }
    }

    [Client]
    void ClientShowScreenMessage(string message)
    {
        ShowScreenMessage(PARTIAL_ANSWER_MESSAGE);
    }

    private void ResetAll()
    {
        foreach(var player in m_Players)
        {
            player.m_RightAnswers = 0;
        }
        ServerUpdateScore();
    }

    //BEGIN HUD FUNCTIONS

    void ShowScreenMessage(string message)
    {
        m_hud.ShowScreenMessage(message);
    }

    [ClientRpc]
    void RpcUpdateTime(float time)
    {
        m_hud.SetTime(time);
    }

    [ClientRpc]
    private void RpcUpdateScore(int player1, int player2)
    {
        m_hud.SetScore(player1, player2);
    }

    [ClientRpc]
    private void RpcSetPartialAnswer(int playerId)
    {
        m_hud.SetPartialAnswer(playerId);
    }

    void UpdateQuestion(string question)
    {
        m_hud.SetQuestion(question);
    }

    void UpdateQuestion(int question)
    {
        m_hud.SetQuestion(question);
    }

    private void SetupPlayerNames()
    {
        m_hud.SetPlayerName(0, m_Players[0].GetName());
        if (m_Players.Count > 1)
        {
            m_hud.SetPlayerName(1, m_Players[1].GetName());
        }
        else
        {
            m_hud.ShowPlayerInfo(1, false);
        }
    }

    //END HUD FUNCTIONS


    //BEGIN FADE FUNCS

    private IEnumerator ClientRoundStartingFade()
    {
        float elapsedTime = 0.0f;
        float wait = m_StartDelay - 0.5f;

        yield return null;

        while (elapsedTime < wait)
        {
            if (m_currentTestList.GetCurrentRound() == 1)
                m_FadingScreen.alpha = 1.0f - (elapsedTime / wait);
            else
                m_EndRoundScreen.alpha = 1.0f - (elapsedTime / wait);

            elapsedTime += Time.deltaTime;

            //sometime, synchronization lag behind because of packet drop, so we make sure our tank are reseted
            if (elapsedTime / wait < 0.5f)
                ResetAll();

            yield return null;
        }
    }

    private IEnumerator ClientRoundEndingFade()
    {
        float elapsedTime = 0.0f;
        float wait = m_EndDelay;
        while (elapsedTime < wait)
        {
            m_EndRoundScreen.alpha = (elapsedTime / wait);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //END FADE FUNCS
}
