using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeModule : MonoBehaviour
{
    private List<ArrangeCell> m_cells;
    public ArrangeCell m_instance;
    public int m_countOfCells;

    // Start is called before the first frame update
    void Start()
    {
        m_cells = new List<ArrangeCell>();
        //SetupSlots(m_countOfCells);
    }

    public int[] GetCurrentAnswers()
    {
        List<int> answers = new List<int>();
        foreach(var cell in m_cells)
        {
            answers.Add((cell && cell.GetCurrentItem()) ? cell.GetCurrentItem().ID : -1);
        }
        return answers.ToArray();
    }

    public void SetupSlots(int count)
    {
        while (m_cells.Count < count)
        {
            ArrangeCell newCell = Instantiate(m_instance, transform);
            m_cells.Add(newCell);
        }
        
        for (int i = count; count < m_cells.Count; ++i)
        {
            m_cells[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < count; ++i)
        {
            m_cells[i].gameObject.SetActive(true);
            m_cells[i].ResetItem();
        }
    }

    public ArrangeCell TryAddToCell(DragNDropHandle handle)
    {
        foreach (var cell in m_cells)
        {
            if (cell.CheckFalling(handle))
            {
                cell.SwapHandles(handle);
                return cell;
            }
        }
        return null;
    }

    public List<ArrangeCell> GetCells()
    {
        return m_cells;
    }
}
