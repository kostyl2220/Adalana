using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrangeCell : MonoBehaviour
{
    private DragNDropHandle m_item;

    public DragNDropHandle GetCurrentItem()
    {
        return m_item;
    }

    public void ResetItem()
    {
        m_item = null;
    }

    public DragNDropHandle SwapHandles(DragNDropHandle newHandle)
    {
        DragNDropHandle oldHandle = m_item;
        if (oldHandle)
        {
            oldHandle.ResetCell();
            oldHandle.SetPos(newHandle.GetStartPos());
        }

        m_item = newHandle;
        RectTransform rect = transform as RectTransform;
        m_item.transform.position = transform.position;// + new Vector3(rect.rect.center.x, rect.rect.center.y);

        return oldHandle;
    }

    public bool CheckFalling(DragNDropHandle handle)
    {  
        RectTransform rect = transform as RectTransform;
        return RectTransformUtility.RectangleContainsScreenPoint(rect, handle.transform.position);
    }
}
