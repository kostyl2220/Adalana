using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCreatorAnswerEntry : MonoBehaviour
{
    public InputField m_text;
    public Toggle m_isRight;

    private TestCreateQuestionEntry m_question;

    public void SetQuestion(TestCreateQuestionEntry question)
    {
        m_question = question;
    }

    public void OnRemoveAnswer()
    {
        m_question.RemoveAnswer(this);
    }
}
