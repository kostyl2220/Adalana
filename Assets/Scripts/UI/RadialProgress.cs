using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadialProgress : RadialFill
{
    private int GetExprerienceForLevel(int level)
    {
        float value = level - 1 + 300.0f * Mathf.Pow(2, (level - 1) / 7.0f);
        return level == 1 ? 0 : (int)(value / 4);
    }

    private int GetCurrentLevel(int experience)
    {
        int outLevel = 1;
        int curExp;
        do
        {
            ++outLevel;
            curExp = GetExprerienceForLevel(outLevel);
        }
        while (experience > curExp);
        return outLevel - 1;
    }

    private void OnEnable()
    {
        Activate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        var experience = GameManager.m_experience;
        var curLevel = GetCurrentLevel(experience);
        var needExp = GetExprerienceForLevel(curLevel + 1);
        var curLevelExp = GetExprerienceForLevel(curLevel);
        var deltaExp = needExp - curLevelExp;
        var curDeltaExp = experience - curLevelExp;

        SetCurrentValue(curDeltaExp, deltaExp);
        levelText.text = curLevel.ToString();
    }
}
