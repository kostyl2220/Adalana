﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutFadeIn : MonoBehaviour
{
    public float m_fadeOutTime = 2.0f;
    public float m_fadeInTime = 2.0f;
    public Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetText(string text)
    {
        m_text.text = text;
        StartFade();
    }

    private void StartFade()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeOut()
    {
        float curTime = 0;
        while (curTime < m_fadeOutTime)
        {
            yield return null;
            curTime += Time.deltaTime;
            transform.localScale = Vector3.one * curTime / m_fadeOutTime;
        }
    }

    IEnumerator FadeIn()
    {
        float curTime = m_fadeInTime;
        while (curTime > 0)
        {
            yield return null;
            curTime -= Time.deltaTime;
            transform.localScale = Vector3.one * curTime / m_fadeInTime;
        }
    }
}
