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

        vars = ShuffleList(vars);

        for (int i = 0; i < count; ++i)
        {
            m_variants[i].gameObject.SetActive(true);
            m_variants[i].ResetCell();
            m_variants[i].Setup(vars[i]);
        }
    }

    private List<E> ShuffleList<E>(List<E> inputList)
    {
        List<E> randomList = new List<E>();

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (inputList.Count > 0)
        {
            randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
            randomList.Add(inputList[randomIndex]); //add it to the new, random list
            inputList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}
