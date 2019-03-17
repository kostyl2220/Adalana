using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Variant
{
    public int ID;
    public string text;

    public Variant(string t, int i)
    {
        text = t;
        ID = i;
    }
}

public class AnswerInitializer : MonoBehaviour
{
    public ArrangeModule m_module;
    public DragNDropHandle m_instance;
    private List<DragNDropHandle> m_variants = new List<DragNDropHandle>();

    public void SetupVariants(List<Variant> vars)
    {
        if (!m_instance)
        {
            return;
        }

        int count = vars.Count;
        while (m_variants.Count < count)
        {
            DragNDropHandle newVariant = Instantiate(m_instance, transform);
            newVariant.SetModule(m_module);
            m_variants.Add(newVariant);
        }

        for (int i = count; count < m_variants.Count; ++i)
        {
            m_variants[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < count; ++i)
        {
            m_variants[i].gameObject.SetActive(true);
            m_variants[i].ResetCell();
            m_variants[i].Setup(vars[i]);
        }
    }
}
