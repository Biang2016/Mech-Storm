using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal class Bullet : PoolObject
{
    public override void PoolRecycle()
    {
        TrailRenderer.enabled = false;
        base.PoolRecycle();
    }

    [SerializeField] private TrailRenderer TrailRenderer;

    public void Move(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor, float duration, float fadeTime, float startWidth, float endWidth)
    {
        StartCoroutine(Co_Move(startPos, endPos, startColor, endColor, duration, fadeTime, startWidth, endWidth));
    }

    IEnumerator Co_Move(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor, float duration, float fadeTime, float startWidth, float endWidth)
    {
        TrailRenderer.startColor = startColor;
        TrailRenderer.endColor = endColor;
        TrailRenderer.time = fadeTime;
        TrailRenderer.startWidth = startWidth;
        TrailRenderer.endWidth = endWidth;

        TrailRenderer.enabled = true;
        transform.position = startPos;

        iTween.MoveTo(gameObject, endPos, duration);
        yield return new WaitForSeconds(duration + 0.1f);
        PoolRecycle();
        yield return null;
    }
}