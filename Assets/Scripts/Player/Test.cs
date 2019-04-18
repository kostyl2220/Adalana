using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestTypes
{
    ARRANGE,
    CHECK
};

[Serializable]
public struct Variant
{
    public int ID;
    public string text;

    public Variant(string t, int i)
    {
        text = t;
        ID = i;
    }
}

[Serializable]
public class Test
{
    public TestTypes m_type; 
    public string m_question;
    public float m_answerTime;
    public Variant[] m_variants;
    public int[] m_answers;
}

[Serializable]
public class TestsList
{
    [SerializeField]
    public List<Test> m_tests;
    [SerializeField]
    public int m_countOfRounds;
    [SerializeField]
    public int m_countQuestionsInRound;

    [NonSerialized]
    private List<Test> m_leftTests;
    [NonSerialized]
    private int m_currentRound;
    [NonSerialized]
    private int m_currentQuestion;

    public TestsList()
    {
        m_tests = new List<Test>();
    }

    public void InitTests()
    {
        m_leftTests = ShuffleList(m_tests);
        m_currentRound = 0;

        while (m_countQuestionsInRound * m_countOfRounds > m_leftTests.Count)
        {
            m_leftTests.AddRange(ShuffleList(m_tests));
        }
    }

    public bool IsRoundAvaliable()
    {
        return m_currentRound < m_countOfRounds;
    }

    public bool HasValidQuestionForRound()
    {
        return m_currentQuestion < m_countQuestionsInRound;
    }

    //starts from 1
    public int GetCurrentRound()
    {
        return m_currentRound + 1;
    }

    //number starts from 1
    public int GetCurrentQuestionNumber()
    {
        return m_currentQuestion + 1;
    }

    public Test GetCurrentTest()
    {
        if (HasValidQuestionForRound())
        {         
            return m_leftTests[m_currentRound * m_countQuestionsInRound + m_currentQuestion];
        }
        return null;
    }

    public void SetNextTest()
    {
        ++m_currentQuestion;
    }

    public bool SetTestsForNextRound()
    {
        ++m_currentRound;
        m_currentQuestion = 0;      
        return m_currentRound < m_countOfRounds;
    }

    private List<E> ShuffleList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();
        List<E> copyList = new List<E>();
        copyList.AddRange(inputList);

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (copyList.Count > 0)
        {
            randomIndex = r.Next(0, copyList.Count); //Choose a random object in the list
            randomList.Add(copyList[randomIndex]); //add it to the new, random list
            copyList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}
