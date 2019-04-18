using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeTestModuleBlock : TestModuleBlock
{
    public override void SetupTest(Test test)
    {
        m_testModule.SetupSlots(test.m_answers.Length);
        m_testModule.SetupVariants(test.m_variants);       
        m_checkStrategy = new DefaultArrangedAnswerCheckStrategy();
    }
}
