using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeTestModuleBlock : TestModuleBlock
{
    public ArrangeModule m_module;
    public AnswerInitializer m_answerInitializer;

    public override int[] GetCurrentAnswers()
    {
        return m_module.GetCurrentAnswers();
    }

    public override void SetupTest(Test test)
    {
        m_module.SetupSlots(test.m_answers.Length);
        m_answerInitializer.SetupVariants(test.m_variants);       
        m_checkStrategy = new DefaultArrangedAnswerCheckStrategy();
    }
}
