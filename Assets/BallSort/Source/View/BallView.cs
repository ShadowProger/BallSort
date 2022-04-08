using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallView : MonoBehaviour
{
    public Ball ball;
    public Tween Tween
    {
        get
        {
            return tween;
        }
        set
        {
            if (tween != null)
            {
                tween.Kill();
            }
            tween = value;
        }
    }

    private Tween tween;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Ball ball)
    {
        this.ball = ball;
        this.ball.view = this;

        spriteRenderer.sprite = GameManager.Instance.GetBallSprite(ball.value);
    }

    public void SetSortingLayer(string layer)
    {
        spriteRenderer.sortingLayerName = layer;
    }
}
