using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerInitializer : MonoBehaviour
{
    public ArrangeModule m_module;
    public DragNDropHandle m_instance;
    public Transform m_gridInstance;
    public Transform m_gridParent;
    private List<Transform> m_gridElements = new List<Transform>();
    private List<DragNDropHandle> m_variants = new List<DragNDropHandle>();

    private List<Variant> m_vars;

    public void SetupVariants(Variant[] vars)
    {
        if (!m_instance)
        {
            return;
        }

        int count = vars.Length;
        while (m_variants.Count < count)
        {
            Transform newPlace = Instantiate(m_gridInstance, m_gridParent);
            m_gridElements.Add(newPlace);
            DragNDropHandle newVariant = Instantiate(m_instance, transform);
            newVariant.SetModule(m_module);
            m_variants.Add(newVariant);
        }

        for (int i = count; i < m_variants.Count; ++i)
        {
            m_variants[i].gameObject.SetActive(false);
        }

        m_vars = ShuffleList(vars);

        StartCoroutine(InitValues());
    }

    private IEnumerator InitValues()
    {
        yield return null;

        for (int i = 0; i < m_vars.Count; ++i)
        {
            m_variants[i].gameObject.SetActive(true);
            m_variants[i].ResetCell();
            m_variants[i].transform.position = m_gridElements[i].position;
            m_variants[i].Setup(m_vars[i]);
        }
    }

    private List<E> ShuffleList<E>(E[] inputList)
    {
        List<E> randomList = new List<E>();
        List<E> copyList = new List<E>();
        copyList.AddRange(inputList);
    
        System.Random r = new System.Random();
        int randomIndex = 0;
        while (copyList.Count > 0)
        {
            randomIndex = r.Next(0, copyList.Count); //Choose a random object in the list
            randomList.Add(copyList[randomIndex]); //add it to the new, random list
            copyList.RemoveAt(randomIndex); //remove to avoid duplicates
        }

        return randomList; //return the new random list
    }
}
