using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarHolder : MonoBehaviour
{
    public GameObject m_starInstance;

    private List<GameObject> m_childrenList;
    private int m_currentActiveStars = 0;

    public void RefreshStars()
    {
        for (int i = 0; i < m_currentActiveStars; ++i)
        {
            m_childrenList[i].SetActive(false);
        }

        m_currentActiveStars = 0;
    }

    public void AddStar()
    {
        HelperFuncs.ASSERT(m_currentActiveStars > m_childrenList.Count, "Can`t activate more stars, than avaliable");

        if (m_currentActiveStars == m_childrenList.Count)
        {
            GameObject newStar = Instantiate(m_starInstance, transform);
            m_childrenList.Add(newStar);
        }

        m_childrenList[m_currentActiveStars].SetActive(true);
        ++m_currentActiveStars;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_childrenList = new List<GameObject>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
