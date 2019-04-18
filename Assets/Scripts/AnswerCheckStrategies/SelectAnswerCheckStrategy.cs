using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAnswerCheckStrategy : IAnswerCheckStrategy
{
    public override int GetScore(int[] rightAnswers, int[] currentAnswers)
    {
        List<int> list1 = new List<int>(rightAnswers);
        int res = 0;
        foreach(var el in currentAnswers)
        {
            res += (list1.Contains(el) ? 1 : -1);
        }
        if (res < 0)
        {
            res = 0;
        }
        return res;
    }
}
