using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Container
{
    public List<Ball> balls = new List<Ball>();
    public int maxBallsCount;

    public ContainerView view;

    public bool IsEmpty => balls.Count == 0;
    public bool IsFull => balls.Count == maxBallsCount;

    public bool IsComplete
    {
        get
        {
            if (balls.Count == 0)
            {
                return true;
            }
            if (balls.Count < maxBallsCount)
            {
                return false;
            }
            int color = balls[0].value;
            foreach (var ball in balls)
            {
                if (ball.value != color)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public bool CanSelect
    {
        get
        {
            if (balls.Count == 0)
            {
                return true;
            }
            if (IsComplete)
            {
                return false;
            }
            return true;
        }
    }

    public Container(int maxBallsCount)
    {
        this.maxBallsCount = maxBallsCount;
    }

    public bool CanPlaceOnTop(Ball ball)
    {
        if (balls.Count >= maxBallsCount)
        {
            return false;
        }

        if (balls.Count > 0)
        {
            var upperBall = balls.Last();
            if (upperBall.value == ball.value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public Ball GetTopBall()
    {
        if (balls.Count > 0)
        {
            return balls.Last();
        }
        else
        {
            return null;
        }
    }

    public Vector3 GetTopPlace()
    {
        return view.places[balls.Count];
    }

    public void RemoveBall(Ball ball)
    {
        balls.Remove(ball);
    }

    public void AddBall(Ball ball)
    {
        if (balls.Count == maxBallsCount)
        {
            return;
        }
        balls.Add(ball);
        ball.container = this;
        ball.view.transform.SetParent(view.transform, true);
        ball.place = balls.Count - 1;
    }
}
