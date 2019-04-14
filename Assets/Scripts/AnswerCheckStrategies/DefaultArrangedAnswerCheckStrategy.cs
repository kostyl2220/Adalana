using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultArrangedAnswerCheckStrategy : IAnswerCheckStrategy
{
    public override int GetScore(int[] rightAnswers, int[] currentAnswers)
    {
        int res = 0;
        for (int i = 0; i < rightAnswers.Length; ++i)
        {
            if (rightAnswers[i] == currentAnswers[i])
            {
                ++res;
            }
        }

        return res;
    }
}
