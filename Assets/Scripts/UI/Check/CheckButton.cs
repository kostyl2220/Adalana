using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckButton : MonoBehaviour
{
    private bool m_selected = false;
    public Image m_image;
    public Text m_text;
    private int ID;

    public void ChangeSelection()
    {
        m_selected = !m_selected;
        EnableImage();
    }

    private void EnableImage()
    {
        m_image.enabled = m_selected;
    }

    public void ResetItem()
    {
        m_selected = false;
        EnableImage();
    }

    public bool IsSelected()
    {
        return m_selected;
    }

    public int GetID()
    {
        return ID;
    }

    public void SetAnswer(Variant answer)
    {
        ResetItem();
        ID = answer.ID;
        if (m_text)
        {
            m_text.text = answer.text;
        }
    }
}
