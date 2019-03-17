using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragNDropHandle : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public int ID;
    public Text m_text;

    private Vector3 m_startPos;
    private Vector3 m_displacement;
    private ArrangeModule m_module;
    private ArrangeCell m_cell;

    public void SetModule(ArrangeModule module)
    {
        m_module = module;
    }

    public void ResetCell()
    {
        if (m_cell)
        {
            m_cell.ResetItem();
            m_cell = null;
        }
    }

    public void SetOnFront()
    {
        Transform oldParent = transform.parent;
        transform.SetParent(transform.parent.parent);
        transform.SetParent(oldParent);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ResetCell();
        SetOnFront();
        m_startPos = transform.position;
        m_displacement = m_startPos - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + m_displacement;
    }

    public Vector3 GetStartPos()
    {
        return m_startPos;
    }

    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
        m_cell = m_module.TryAddToCell(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_cell = m_module.TryAddToCell(this);
    }

    public void Setup(Variant var)
    {
        ID = var.ID;
        if (m_text)
        {
            m_text.text = var.text;
        }
    }
}
