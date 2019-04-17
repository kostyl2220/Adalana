using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum MessageType
{
    ClientReady = 1002,
    ClientAnswer
}

public class ClientReadyMessage : MessageBase
{
    public short ID;
}

public class ClientAnswerMessage : MessageBase
{
    public short ID;
    public int[] answerList;
}

