using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Window : MonoBehaviour
{
    public bool IsOpen { get; set; }

    public virtual void Init()
    {

    }

    public virtual void Open()
    {
        IsOpen = true;
    }

    public virtual void Close()
    {
        IsOpen = false;
    }
}
