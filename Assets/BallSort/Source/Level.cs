using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public LevelSettings settings;
    public List<Container> containers = new List<Container>();
    public bool isAdditionalContainer;

    public static int containerSize;

    public bool CheckWin()
    {
        foreach (var container in containers)
        {
            if (!container.IsComplete)
            {
                return false;
            }
        }
        return true;
    }

    //public bool CheckLose()
    //{
    //    foreach (var container in containers)
    //    {
    //        if (container.IsEmpty)
    //        {
    //            return false;
    //        }
    //    }
    //}

    public void Load(LevelSettings settings)
    {
        Clear();
        this.settings = settings;
        containerSize = settings.containerSize;

        for (int i = 0; i < settings.containers.Count; i++)
        {
            AddContainer(settings.containers[i].balls);
        }

        if (isAdditionalContainer)
        {
            AddContainer();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < containers.Count; i++)
        {
            var container = containers[i];
            container.balls.Clear();
        }
        containers.Clear();
    }

    public Container AddContainer(List<int> balls = null)
    {
        var container = new Container(settings.containerSize);
        if (balls != null)
        {
            for (int j = 0; j < balls.Count; j++)
            {
                var ball = new Ball(balls[j]);
                container.balls.Add(ball);
            }
        }
        containers.Add(container);

        return container;
    }
}
