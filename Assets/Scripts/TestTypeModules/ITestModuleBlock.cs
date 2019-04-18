using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestModuleBlock : MonoBehaviour
{
    public GameObject m_testScene;
    public IAnswerCheckStrategy m_checkStrategy;
    public ITestModule m_testModule;

    public int[] GetCurrentAnswers()
    {
        return m_testModule.GetCurrentAnswers();
    }

    public int GetScore(int[] rightAnswers, int[] currentAnswers)
    {
        return m_checkStrategy.GetScore(rightAnswers, currentAnswers);
    }

    public void ActivateScene(bool activate)
    {
        m_testScene.SetActive(activate);
    }

    public abstract void SetupTest(Test test);
}
