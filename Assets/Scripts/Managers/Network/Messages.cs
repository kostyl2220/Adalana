using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum MessageType
{
    ClientReady = 1002,
    ClientAnswer
}

public class ClientMessage : MessageBase
{
    public int ID;

    public ClientMessage()
    {

    }

    public ClientMessage(int id)
    {
        ID = id;
    }
}


public class ClientReadyMessage : ClientMessage
{
    public ClientReadyMessage() { }

    public ClientReadyMessage(int id) : base(id)
    {
    }
}

public class ClientAnswerMessage : ClientMessage
{
    public string test;
    public int[] answerList;

    public ClientAnswerMessage()
    {
    }

    public override string ToString()
    {
        return ("ID " + ID + " list " + answerList.ToString() + test);
    }

    public ClientAnswerMessage(int id, int[] res) 
    {
        ID = id;
        answerList = res;
    }
}

