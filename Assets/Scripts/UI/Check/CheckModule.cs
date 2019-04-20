using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheckModule : ITestModule
{
    public CheckButton m_instance;
    private List<CheckButton> m_buttons;
    private int m_currentCount;

    // Start is called before the first frame update
    void Start()
    {
        m_buttons = new List<CheckButton>();
    }

    public override int[] GetCurrentAnswers()
    {
        List<int> res = new List<int>();

        for (int i = 0; i < m_currentCount; ++i)
        {
            if (m_buttons[i].IsSelected())
            {
                res.Add(m_buttons[i].GetID());
            }
        }

        return res.ToArray();
    }

    public override void SetupVariants(Variant[] variants)
    {
        System.Random rnd = new System.Random();
        Variant[] copy = (new List<Variant>(variants)).OrderBy(x => rnd.Next()).ToArray();
        for (int i = 0; i < copy.Length; ++i)
        {
            m_buttons[i].SetAnswer(copy[i]);
        }
    }

    public override void SetupSlots(int count)
    {
        m_currentCount = count;
        while (m_buttons.Count < count)
        {
            CheckButton newCell = Instantiate(m_instance, transform);
            m_buttons.Add(newCell);
        }

        for (int i = count; count < m_buttons.Count; ++i)
        {
            m_buttons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < count; ++i)
        {
            m_buttons[i].gameObject.SetActive(true);
            m_buttons[i].ResetItem();
        }
    }
}
