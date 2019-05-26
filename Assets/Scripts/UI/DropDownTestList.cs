using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DropDownTestList : MonoBehaviour
{
    public Dropdown m_dropdown; 

    // Start is called before the first frame update
    void Start()
    {
        if (!m_dropdown)
        {
            m_dropdown = gameObject.GetComponent<Dropdown>();
        }
        if (m_dropdown)
        {
            m_dropdown.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }
    }

    private void OnEnable()
    {
        if (m_dropdown)
        {
            InitTests();
        }
    }

    private void InitTests()
    {
        string[] filePaths = Directory.GetFiles(TestReader.GetTestFolderPath(), "*" + TestReader.JSON_TYPE);
        m_dropdown.ClearOptions();
        List<string> result = new List<string>();
        foreach(var path in filePaths)
        {
            result.Add(Path.GetFileNameWithoutExtension(path));
        }
        m_dropdown.AddOptions(result);
    }

    private void OnValueChanged()
    {
        string curValue = m_dropdown.options[m_dropdown.value].text;
        GameManager.m_testListName = curValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
