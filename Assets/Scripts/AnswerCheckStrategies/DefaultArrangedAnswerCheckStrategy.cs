using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultArrangedAnswerCheckStrategy : IAnswerCheckStrategy
{
    private ArrangeModule m_module;

    public DefaultArrangedAnswerCheckStrategy(ArrangeModule module)
    {
        m_module = module;
    }

    public override int GetScore(List<int> answers)
    {
        int res = 0;
        for (int i = 0; i < answers.Count; ++i)
        {
            ArrangeCell cell = m_module.GetCells()[i];
            if (cell.GetCurrentItem() && cell.GetCurrentItem().ID == answers[i])
            {
                ++res;
            }
        }

        return res;
    }
}
