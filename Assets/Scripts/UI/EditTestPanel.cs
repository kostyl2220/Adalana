using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTestPanel : MonoBehaviour
{
    public GameObject m_testEditor;
    public InputField m_testNameT;
    public InputField m_roundCount;
    public InputField m_questionsInRound;
    public GameObject m_questionArea;
    public TestCreateQuestionEntry m_instance;

    private List<TestCreateQuestionEntry> m_questions = new List<TestCreateQuestionEntry>();

    private void OnEnable()
    {

    }

    public void ResetTest()
    {
        foreach (var question in m_questions)
        {
            Destroy(question.gameObject);
        }
        m_questions.Clear();
        m_testNameT.text = "";
        m_questionsInRound.text = "";
        m_roundCount.text = "";
    }

    internal void SetTestName(string name)
    {
        TestsList testsList = TestReader.GetTestList(name);
        ResetTest();
        EditTest(testsList);
    }

    public void OnBack()
    {
        if (m_testEditor)
        {
            m_testEditor.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    internal void OnRemoveQuestion(TestCreateQuestionEntry testCreateQuestionEntry)
    {
        m_questions.Remove(testCreateQuestionEntry);
        Destroy(testCreateQuestionEntry.gameObject);
    }

    public void AddQuestionBtn()
    {
        TestCreateQuestionEntry question = Instantiate(m_instance, m_questionArea.transform);
        question.SetPanel(this);
        m_questions.Add(question);
    }

    private void EditTest(TestsList testList)
    {
        m_testNameT.text = testList.m_name;
        m_roundCount.text = testList.m_countOfRounds.ToString();
        m_questionsInRound.text = testList.m_countQuestionsInRound.ToString();
        foreach (var question in testList.m_tests)
        {
            AddQuestion(question);
        }
    }

    private void AddQuestion(Test test)
    {
        TestCreateQuestionEntry question = Instantiate(m_instance, m_questionArea.transform);
        question.SetPanel(this);
        question.m_timeField.text = test.m_answerTime.ToString();
        question.m_testType.value = (int)test.m_type;
        question.m_questionNameField.text = test.m_question;
        for (int i = 0; i < test.m_variants.Length; ++i)
        {
            bool isRight = false;
            for (int j = 0; j < test.m_answers.Length; ++j)
            {
                if (test.m_variants[i].ID == test.m_answers[j])
                {
                    isRight = true;
                    break;
                }
            }
            question.AddAnswer(test.m_variants[i].text, isRight);
        }

        m_questions.Add(question);
    }

    public void OnSaveTest()
    {
        TestsList testList = new TestsList();
        testList.m_name = m_testNameT.text;
        testList.m_countOfRounds = int.Parse(m_roundCount.text);
        testList.m_countQuestionsInRound = int.Parse(m_questionsInRound.text);
        foreach (var question in m_questions)
        {
            Test test = new Test();
            test.m_question = question.m_questionNameField.text;
            test.m_answerTime = float.Parse(question.m_timeField.text);
            test.m_type = (TestTypes)question.m_testType.value;

            var answerList = question.GetAnswersList();
            test.m_variants = new Variant[answerList.Count];

            int totalCountOfRight = 0;
            for (int i = 0; i < answerList.Count; ++i)
            {
                if (answerList[i].m_isRight.isOn)
                {
                    ++totalCountOfRight;
                }
            }

            test.m_answers = new int[totalCountOfRight];
            int countOfRight = 0;
            for (int i = 0; i < answerList.Count; ++i)
            {
                Variant variant = new Variant(answerList[i].m_text.text, i);
                test.m_variants[i] = variant;
                if (answerList[i].m_isRight.isOn)
                {
                    test.m_answers[countOfRight] = i;
                    ++countOfRight;
                }
            }

            testList.m_tests.Add(test);
        }

        TestReader.SaveTestList(testList);
        OnBack();
    }
}
