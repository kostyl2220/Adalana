using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestModuleBlock : MonoBehaviour
{
    public GameObject m_testScene;
    protected IAnswerCheckStrategy m_checkStrategy;

    public abstract int[] GetCurrentAnswers();

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
