using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerView : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public string skinName;
    public int size;
    public SpriteRenderer containerSprite;
    public SpriteRenderer shadow;
    public SpriteRenderer glow;
    [HideInInspector] public Vector3 top;
    [HideInInspector] public List<Vector3> places = new List<Vector3>();
    [HideInInspector] public Transform balls;

    public const float BUFFER = 100;
    public static float height;

    public Container container;

    private Tween glowTween;
    private Color glowColor;

    public Sprite Sprite => containerSprite.sprite;

    public Vector3 GlobalTop
    {
        get
        {
            return transform.position + top;
        }
    }

    private void Awake()
    {
        glowColor = glow.color;
        glow.gameObject.SetActive(false);

        var topTr = transform.Find("Top");
        top = topTr.localPosition;
        Destroy(topTr.gameObject);
        balls = transform.Find("Balls");
        foreach (Transform ball in balls)
        {
            places.Add(ball.localPosition);
            Destroy(ball.gameObject);
        }
    }

    public void Init(Container container)
    {
        this.container = container;
        this.container.view = this;

        Resize();
    }

    public void Resize()
    {
        float spriteWidth = containerSprite.sprite.rect.width;
        float spriteHeight = containerSprite.sprite.rect.height;

        height = spriteHeight + BUFFER * 2;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.SelectContainer(container);
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void DoGlow()
    {
        if (glowTween != null)
        {
            glowTween.Kill();
        }

        var startColor = glowColor;
        var endColor = glowColor;
        startColor.a = 0f;
        glow.color = startColor;
        glow.gameObject.SetActive(true);

        float durationIn = 0.5f;
        float durationOut = 0.5f;

        var sequence = DOTween.Sequence();
        sequence.Append(glow.DOColor(endColor, durationIn).SetEase(Ease.InCubic));
        sequence.Append(glow.DOColor(startColor, durationOut).SetEase(Ease.InQuad));
        sequence.OnComplete(() => { glow.gameObject.SetActive(false); });
        sequence.OnKill(() => { glow.gameObject.SetActive(false); });

        glowTween = sequence;
    }
}
