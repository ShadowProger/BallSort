using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver
{
    LevelSnap levelSnap;
    static int containerSize;
    static int nextId;

    public bool CanSolve(Level level)
    {
        nextId = 0;
        containerSize = level.settings.containerSize;

        levelSnap = new LevelSnap(level);

        return false;
    }

    private static int GetNextId()
    {
        return nextId++;
    }

    #region sealed
    class Container
    {
        public List<Ball> balls = new List<Ball>();

        public bool IsEmpty => balls.Count == 0;
        public bool IsComplete
        {
            get 
            {
                if (IsEmpty)
                {
                    return true;
                }
                if (balls.Count < containerSize)
                {
                    return false;
                }
                int color = balls[0].color;
                foreach (var ball in balls)
                {
                    if (ball.color != color)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    class Ball
    {
        public int id;
        public int color;

        public Ball(int color)
        {
            this.color = color;
            id = GetNextId();
        }
    }

    class LevelSnap
    {
        public List<Container> containers = new List<Container>();

        public LevelSnap(Level level)
        {
            foreach (var cont in level.containers)
            {
                Container container = new Container();

                foreach (var b in cont.balls)
                {
                    Ball ball = new Ball(b.value);
                    container.balls.Add(ball);
                }

                containers.Add(container);
            }
        }
    }
    #endregion
}
