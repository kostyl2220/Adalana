using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeTestModule : TestModule
{
    public ArrangeModule m_module;
    public AnswerInitializer m_answerInitializer;

    public override void SetupTest(List<Variant> vars, int answerCount = 1)
    {
        m_module.SetupSlots(vars.Count);
        m_answerInitializer.SetupVariants(vars);       
        m_checkStrategy = new DefaultArrangedAnswerCheckStrategy(m_module);
    }
}
