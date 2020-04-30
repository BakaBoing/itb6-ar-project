using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScroll : MonoBehaviour
{
    public Scrollbar scrollbar;
    private void Start()
    {
        SetScroll.Instance.AddScrollbar(scrollbar);
    }

    private void OnDestroy()
    {
        SetScroll.Instance.scrollbars.Remove(scrollbar);
    }
}
