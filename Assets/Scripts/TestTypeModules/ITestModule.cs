using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestModule : MonoBehaviour
{
    public GameObject m_testScene;
    protected IAnswerCheckStrategy m_checkStrategy;

    public int GetScore(List<int> answers)
    {
        return m_checkStrategy.GetScore(answers);
    }

    public void ActivateScene(bool activate)
    {
        m_testScene.SetActive(activate);
    }

    public abstract void SetupTest(List<Variant> vars, int answerCount = 1);
}
