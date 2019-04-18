using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeModule : ITestModule
{
    public AnswerInitializer m_initializer;
    private List<ArrangeCell> m_cells;
    public ArrangeCell m_instance;
    public int m_countOfCells;

    // Start is called before the first frame update
    void Start()
    {
        m_cells = new List<ArrangeCell>();
    }

    public override int[] GetCurrentAnswers()
    {
        List<int> answers = new List<int>();
        for (int i = 0; i < m_countOfCells; ++i)
        {
            ArrangeCell cell = m_cells[i];
            answers.Add((cell && cell.GetCurrentItem()) ? cell.GetCurrentItem().ID : -1);
        }
        return answers.ToArray();
    }

    public override void SetupSlots(int count)
    {
        m_countOfCells = count;
        while (m_cells.Count < m_countOfCells)
        {
            ArrangeCell newCell = Instantiate(m_instance, transform);
            m_cells.Add(newCell);
        }
        
        for (int i = m_countOfCells; m_countOfCells < m_cells.Count; ++i)
        {
            m_cells[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < m_countOfCells; ++i)
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

    public override void SetupVariants(Variant[] vars)
    {
        if (m_initializer)
        {
            m_initializer.SetupVariants(vars);
        }
    }
}
