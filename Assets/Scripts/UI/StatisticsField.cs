﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsField : MonoBehaviour
{
    public Text m_number;
    public Text m_statName;
    public Text m_statValue;

    public void SetNumber(int number)
    {
        if (m_number)
        {
            m_number.text = number + ".";
        }
    }

    public void SetName(string name)
    {
        if (m_statName)
        {
            m_statName.text = name;
        }
    }

    public void SetValue(string value)
    {
        if (m_statValue)
        {
            m_statValue.text = value;
        }
    }
}
