﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAnswerCheckStrategy 
{
    public abstract int GetScore(int[] rightAnswers, int[] currentAnswers);
}
