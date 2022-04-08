using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball
{
    public int value;
    public BallView view;
    public Container container;
    public int place;

    public Ball(int value)
    {
        this.value = value;
    }
}
