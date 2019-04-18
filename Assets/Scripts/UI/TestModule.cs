using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITestModule : MonoBehaviour
{
    public abstract void SetupSlots(int count);

    public abstract int[] GetCurrentAnswers();

    public abstract void SetupVariants(Variant[] m_variants);
}
