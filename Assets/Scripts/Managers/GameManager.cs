using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using System;
using PlayFab.ClientModels;
using PlayFab;

public class GameManager : NetworkBehaviour
{
    static string FILE_NOT_FOUND = "CAN'T FIND FILE";
    static string PARTIAL_ANSWER_MESSAGE = "PARTIALLY ANSERED";
    static string WIN_MESSAGE = "YOU WIN";
    static string LOSE_MESSAGE = "YOU LOSE";
    static string DRAW_MESSAGE = "DRAW";

    static string PLAYFAB_WIN_COUNT = "WinCount";
    static string PLAYFAB_LOSE_COUNT = "LoseCount";
    static string PLAYFAB_DRAW_COUNT = "DrawCount";
    static string PLAYFAB_TOTAL_POINTS = "TotalPoints";

    static public short INVALID_PLAYER_ID = -1;
    static private int INVALID_PARTIAL_SELECTED = -1;
    static public int INVALID_SCORE = -1;
    static public GameManager s_Instance;

    //this is static so tank can be added even withotu the scene loaded (i.e. from lobby)
    static public List<PlayerManager> m_Players = new List<PlayerManager>();             // A collection of managers for enabling and disabling different aspects of the tanks.
    static public string m_testListName = "test_f";

    public int m_NumRoundsToWin = 5;          // The number of rounds a single player has to win to win the game.
    public float m_RoundStartDelay = 0.5f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_RoundEndDelay = 0.5f;             // The delay between the end of RoundPlaying and RoundEnding phases.
    public float m_LoadingWaitDelay = 0.1f;           // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_endGameWaitingTime = 3.0f;

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

    private WaitForSeconds m_RoundStartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_LoadingWait;
    private WaitForSeconds m_RoundEndWait;           // Used to have a delay whilst the round or game ends.
    private int m_RoundWinnerId;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private int m_GameWinnerId;           // Reference to the winner of the game.  Used to make an announcement of who won.
    private short m_localPlayerID = INVALID_PLAYER_ID;

    private List<TestModuleBlock> m_testModules;
    private TestModuleBlock m_currentTest;
    private TestsList m_currentTestList;


    void Awake()
    {
        s_Instance = this;
    }

    private void Hack_SetupTestList()
    {
        m_currentTestList = new TestsList();

        Variant[] vars1 = 
        {
             new Variant("Hello", 1)
            , new Variant("World", 2)
            , new Variant("How are", 3)
            , new Variant("you?", 4)
        };

        int[] answers1 = { 1, 2, 3, 4 };

        Test test1 = new Test
        {
            m_type = TestTypes.ARRANGE,
            m_question = "Combine first program correctly",
            m_answerTime = 20.0f,
            m_variants = vars1,
            m_answers = answers1
        };

        Variant[] vars2 = 
        {
             new Variant("Lol", 1)
            , new Variant("Kek", 2)
            , new Variant("Chebu", 3)
            , new Variant("Rek", 4)
        };

        int[] answers2 = { 1, 2, 3, 4 };

        Test test2 = new Test
        {
            m_type = TestTypes.ARRANGE,
            m_question = "Piu",
            m_answerTime = 20.0f,
            m_variants = vars2,
            m_answers = answers2
        };

        m_currentTestList.m_countOfRounds = 2;
        m_currentTestList.m_countQuestionsInRound = 5;
        m_currentTestList.m_tests.Add(test1);
        m_currentTestList.m_tests.Add(test2);
    }

    [ClientRpc]
    public void RpcSetupGame(int totalRounds)
    {
        SetupPlayerNames();
        SetupModules();
        m_hud.SetTotalRounds(totalRounds);
    }

    private void SetupLocalPlayerID()
    {
        for (short i = 0; i < m_Players.Count; ++i)
        {
            if (m_Players[i].IsLocalPlayer())
            {
                m_localPlayerID = i;
                return;
            }
        }
    }

    private void SetupModules()
    {
        m_testModules = new List<TestModuleBlock>();
        foreach (Transform module in transform)
        {
            ArrangeTestModuleBlock atm = module.GetComponent<ArrangeTestModuleBlock>();
            if (atm)
            {
                m_testModules.Add(atm);
            }

            CheckTestModuleBlock ctm = module.GetComponent<CheckTestModuleBlock>();
            if (ctm)
            {
                m_testModules.Add(ctm);
            }
        }
    }

    [ServerCallback]
    private void Start()
    {
        // Create the delays so they only have to be made once.
        m_RoundStartWait = new WaitForSeconds(m_RoundStartDelay);
        m_RoundEndWait = new WaitForSeconds(m_RoundEndDelay);
        m_LoadingWait = new WaitForSeconds(m_LoadingWaitDelay);

        //TODO use Command function
        NetworkServer.RegisterHandler((short)MessageType.ClientReady, OnClientReady);
        NetworkServer.RegisterHandler((short)MessageType.ClientAnswer, OnClientAnswered);

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
    static public void AddPlayer(GameObject tank, int playerNum, string name)
    {
        PlayerManager tmp = new PlayerManager();
        tmp.m_Instance = tank;
        tmp.m_PlayerNumber = playerNum;
        tmp.m_PlayerName = name;
        tmp.Setup();
        m_Players.Add(tmp);
    }

    public void RemovePlayer(GameObject tank)
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

    [ClientRpc]
    public void RpcCheckReady()
    {
        if (m_localPlayerID != INVALID_PLAYER_ID)
        {
            return;
        }

        SetupLocalPlayerID();

        if (!isServer)
        {
            ClientReadyMessage msg = new ClientReadyMessage();
            msg.ID = m_localPlayerID;
            NetworkManager.singleton.client.Send((short)MessageType.ClientReady, msg);
        }
        else
        {
            m_Players[m_localPlayerID].m_loadReady = true;
        }
    }

    // This is called from start and will run each phase of the game one after another. ONLY ON SERVER (as Start is only called on server)
    private IEnumerator GameLoop()
    {
        while (m_Players.Count < 1)//LobbyManager.s_Singleton.numPlayers)
            yield return null;

        while (!AllPlayersReady())
        {
            RpcCheckReady();
            yield return m_LoadingWait;
        }

        //Hack_SetupTestList();
        //TestReader.SaveTestList("test", m_currentTestList);

        m_currentTestList = TestReader.GetTestList(m_testListName);
        if (m_currentTestList == null)
        {
            RpcShowMessageAll(FILE_NOT_FOUND);
            yield return new WaitForSeconds(m_endGameWaitingTime);
            LobbyManager.s_Singleton.ServerReturnToLobby();
        }
        m_currentTestList.InitTests();

        RpcSetupGame(m_currentTestList.m_countOfRounds);

        while (m_currentTestList.IsRoundAvaliable())
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundStarting());
            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());
            // Once execution has returned here, run the 'RoundEnding' coroutine.
            yield return StartCoroutine(RoundEnding());

            m_currentTestList.SetTestsForNextRound();
        }

        m_GameWinnerId = GetGameWinnerId();
        RpcEndGame(m_GameWinnerId);
        UpdatePlayfabScore();
        yield return new WaitForSeconds(m_endGameWaitingTime);
        LobbyManager.s_Singleton.ServerReturnToLobby();
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

    private IEnumerator RoundStarting()
    {
        ResetAll();
        //we notify all clients that the round is starting
        RpcRoundStarting(m_currentTestList.GetCurrentRound());

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_RoundStartWait;
    }

    [ClientRpc]
    void RpcRoundStarting(int roundNumber)
    {
        // Increment the round number and display text showing the players what round it is.
        m_hud.SetRound(roundNumber);
    }

    private IEnumerator RoundPlaying()
    {     
        while (m_currentTestList.HasValidQuestionForRound())
        {
            Test currentTest = m_currentTestList.GetCurrentTest();
            RpcTestPlaying(currentTest, m_currentTestList.GetCurrentQuestionNumber());

            m_answerSelected = false;
            float remainingTime = currentTest.m_answerTime;
            // While there is not one tank left...
            while (remainingTime > 0 && !m_answerSelected)
            {
                RpcUpdateTime(remainingTime);
                remainingTime -= Time.deltaTime;
                // ... return on the next frame.
                yield return null;
            }
            RpcTestAnswered();
            m_currentTestList.SetNextTest();
            m_partialAnsweredPlayerId = INVALID_PARTIAL_SELECTED;
            RpcSetPartialAnswer(m_partialAnsweredPlayerId);           
            yield return null;
        }
    }

    [ClientRpc]
    void RpcTestPlaying(Test currentTest, int questionNum)
    {
        m_currentTest = m_testModules[(int)currentTest.m_type];
        m_currentTest.ActivateScene(true);
        m_currentTest.SetupTest(currentTest);

        UpdateQuestion(currentTest.m_question);
        UpdateQuestion(questionNum);
    }

    [ClientRpc]
    void RpcTestAnswered()
    {
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
        yield return m_RoundEndWait;
    }

    [ClientRpc]
    private void RpcRoundEnding(int roundWinnerId)
    {
        if (m_Players.Count > 1)
        {
            m_hud.AddStar(roundWinnerId);
        }
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
            UpdatePlayerStatistics(PLAYFAB_DRAW_COUNT);
        }
        else
        {
            var isWinner = winnerId == m_localPlayerID;
            ShowScreenMessage(isWinner ? WIN_MESSAGE : LOSE_MESSAGE);
            UpdatePlayerStatistics(isWinner ? PLAYFAB_WIN_COUNT : PLAYFAB_LOSE_COUNT);
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

    private void CheckAnswers(int playerID, int[] currentAnswers)
    {
        if (playerID == m_partialAnsweredPlayerId)
        {
            return;
        }

        PlayerManager player = m_Players[playerID];
        int score = m_currentTest.GetScore(m_currentTestList.GetCurrentTest().m_answers, currentAnswers);
        player.AddScore(score);
        m_answerSelected = true;
        ServerUpdateScore();

        if (m_Players.Count < 2)
        {
            return;
        }

        //partialAnswer
        if (score != m_currentTestList.GetCurrentTest().m_answers.Length)
        {
            if (m_partialAnsweredPlayerId == INVALID_PARTIAL_SELECTED)
            {
                m_answerSelected = false;
            }
            m_partialAnsweredPlayerId = playerID;
            RpcSetPartialAnswer(playerID);
            RpcSetPlayerMessageLocal(playerID, PARTIAL_ANSWER_MESSAGE);
        }
    }

    //on server
    private void ServerUpdateScore()
    {
        RpcUpdateScore(m_Players[0].m_RightAnswers, m_Players.Count == 2 ? m_Players[1].m_RightAnswers : INVALID_SCORE);
    }

    //on server
    [ClientRpc]
    public void RpcSetPlayerMessageLocal(int playerID, string message)
    {
        if (m_localPlayerID == playerID)
        {
            ShowScreenMessage(message);
        }
    }

    private void ResetAll()
    {
        foreach(var player in m_Players)
        {
            player.Reset();
        }
        ServerUpdateScore();
    }

    //BEGIN BUTTON PRESS FUNCS

    public void QuitGameButtonPressed()
    {
        LobbyManager.s_Singleton.ServerReturnToLobby();
    }

    public void CheckAnswersButtonPressed()
    {
        if (isServer)
        {
            CheckAnswers(m_localPlayerID, m_currentTest.GetCurrentAnswers());
        }
        else
        {
            ClientAnswerMessage msg = new ClientAnswerMessage();
            msg.ID = m_localPlayerID;
            msg.answerList = m_currentTest.GetCurrentAnswers();
            NetworkManager.singleton.client.Send((short)MessageType.ClientAnswer, msg);
        }
    }

    //END BUTTON PRESS FUNCS

    //BEGIN HUD FUNCTIONS

    void ShowScreenMessage(string message)
    {
        m_hud.ShowScreenMessage(message);
    }

    [ClientRpc]
    private void RpcShowMessageAll(string reason)
    {
        ShowScreenMessage(reason);
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
        float wait = m_RoundStartDelay - 0.5f;

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
        float wait = m_RoundEndDelay;
        while (elapsedTime < wait)
        {
            m_EndRoundScreen.alpha = (elapsedTime / wait);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //END FADE FUNCS

    //START ADDITIONAL HANDLER FUNCS DUE TO UNWORKING COMMAND FUNC

    void OnClientReady(NetworkMessage netMsg)
    {
        ClientReadyMessage msg = netMsg.ReadMessage<ClientReadyMessage>();
        m_Players[msg.ID].m_loadReady = true;
    }

    void OnClientAnswered(NetworkMessage netMsg)
    {
        ClientAnswerMessage msg = netMsg.ReadMessage<ClientAnswerMessage>();
        CheckAnswers(msg.ID, msg.answerList);      
    }

    //END ADDINITONAL HANDLER FUNCS

    //START PLAYFAB FUNCS

    private void UpdatePlayfabScore()
    {
        int[] results = new int[m_Players.Count];
        for (int i = 0; i < m_Players.Count; ++i)
        {
            results[i] = m_Players[i].m_totalScore;
        }
        RpcUpdatePlayfabScore(results);
    }

    [ClientRpc]
    private void RpcUpdatePlayfabScore(int[] scores)
    {
        int localScore = scores[m_localPlayerID];
        UpdatePlayerStatistics(PLAYFAB_TOTAL_POINTS, localScore);
    }

    private void UpdatePlayerStatistics(string statistics, int value = 1)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate { StatisticName = statistics, Value = value },
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });
    }

    //END PLAYFAB FUNCS
}
