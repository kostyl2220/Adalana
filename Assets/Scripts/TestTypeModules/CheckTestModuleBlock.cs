using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTestModuleBlock : TestModuleBlock
{
    public override void SetupTest(Test test)
    {
        m_testModule.SetupSlots(test.m_variants.Length);
        m_testModule.SetupVariants(test.m_variants);
        m_checkStrategy = new SelectAnswerCheckStrategy();
    }
}
