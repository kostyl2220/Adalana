using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFuncs
{
    static public void ASSERT(bool statement, string message = "")
    {
        if (statement)
        {
            Debug.LogError(message);
        }
    }
}
