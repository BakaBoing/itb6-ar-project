using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScroll : MonoBehaviour
{
    public static SetScroll Instance;
    public List<Scrollbar> scrollbars;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            setScrolling(1f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setScrolling(0.5f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            setScrolling(0f);
        }
    }

    public void setScrolling(float f)
    {
        f = Math.Max(0, Math.Min(1, f));
        foreach (var scrollbar in scrollbars)
        {
            scrollbar.value = f;
        }
    }
}
