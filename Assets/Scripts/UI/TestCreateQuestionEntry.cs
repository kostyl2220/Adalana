using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCreateQuestionEntry : MonoBehaviour
{
    public InputField m_timeField;
    public InputField m_questionNameField;
    public Dropdown m_testType;
    public GameObject m_questionArea;
    public TestCreatorAnswerEntry m_answerEntry;

    private EditTestPanel m_panel;

    private static List<string> types =
        new List<string>{ "Arrange", "Choise"};

    private void Start()
    {
        SetupVariants();
    }

    public void SetupVariants()
    {
        m_testType.ClearOptions();
        m_testType.AddOptions(types);
    }

    private List<TestCreatorAnswerEntry> m_answers = new List<TestCreatorAnswerEntry>();

    public void SetPanel(EditTestPanel panel)
    {
        m_panel = panel;
    }

    internal void RemoveAnswer(TestCreatorAnswerEntry testCreatorAnswerEntry)
    {
        m_answers.Remove(testCreatorAnswerEntry);
        Destroy(testCreatorAnswerEntry.gameObject);
    }

    public void OnAddQuestionBtn()
    {
        TestCreatorAnswerEntry answer = Instantiate(m_answerEntry, m_questionArea.transform);
        answer.SetQuestion(this);
        m_answers.Add(answer);
    }

    public void AddAnswer(string answerName, bool isRight)
    {
        TestCreatorAnswerEntry answer = Instantiate(m_answerEntry, m_questionArea.transform);
        answer.SetQuestion(this);
        answer.m_isRight.isOn = isRight;
        answer.m_text.text = answerName;
        m_answers.Add(answer);
    }

    public List<TestCreatorAnswerEntry> GetAnswersList()
    {
        return m_answers;
    }

    public void OnRemoveQuestion()
    {
        m_panel.OnRemoveQuestion(this);
    }
}
