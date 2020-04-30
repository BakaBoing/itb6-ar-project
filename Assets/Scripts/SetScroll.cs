using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScroll : MonoBehaviour
{
    public static SetScroll Instance;
    public List<Scrollbar> scrollbars = new List<Scrollbar>();
    private float currentScroll = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        setScrolling(currentScroll);
    }

    public void AddScrollbar(Scrollbar scrollbar)
    {
        scrollbars.Add(scrollbar);
        scrollbar.value = currentScroll;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            scrollUp(0.01f);
        }    
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setScrolling(0.5f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            scrollDown(0.01f);
        }
    }

    public void setScrolling(float f)
    {
        f = Math.Max(0, Math.Min(1, f));
        currentScroll = f;
        foreach (var scrollbar in scrollbars)
        {
            scrollbar.value = f;
        }
    }

    public void scrollUp(float amount)
    {
        setScrolling(currentScroll + amount);
    }
    
    public void scrollDown(float amount)
    {
        setScrolling(currentScroll - amount);
    }
}
