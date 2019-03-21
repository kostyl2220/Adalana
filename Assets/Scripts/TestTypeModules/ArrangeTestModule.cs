using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeTestModule : TestModule
{
    public ArrangeModule m_module;
    public AnswerInitializer m_answerInitializer;

    public override void SetupTest(Test test)
    {
        m_module.SetupSlots(test.m_answers.Count);
        m_answerInitializer.SetupVariants(test.m_variants);       
        m_checkStrategy = new DefaultArrangedAnswerCheckStrategy(m_module);
    }
}
