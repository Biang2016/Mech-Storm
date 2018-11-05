using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class TransitManager : MonoSingleton<TransitManager>
{
    private TransitManager()
    {
    }

    void Awake()
    {
        TransitPanel.color = new Color(0, 0, 0, 0);
        TransitPanel.raycastTarget = false;
        UpperTransitPanel.transform.position = UpperTransitHidePos.position;
        BottomTransitPanel.transform.position = BottomTransitHidePos.position;
    }

    [SerializeField] private Image TransitPanel;

    enum TransitState
    {
        Hide,
        Show,
        Trans,
    }

    private TransitState M_TransitState;

    [SerializeField] private Image UpperTransitPanel;
    [SerializeField] private Transform UpperTransitHidePos;
    [SerializeField] private Transform UpperTransitShutPos;
    [SerializeField] private Image BottomTransitPanel;
    [SerializeField] private Transform BottomTransitHidePos;
    [SerializeField] private Transform BottomTransitShutPos;

    public void ShowTransit(Color color, float duration)
    {
        if (M_TransitState == TransitState.Show) return;
        StartCoroutine(Co_ShowTransit(color, duration));
    }

    public void HideTransit(Color color, float duration)
    {
        if (M_TransitState == TransitState.Hide) return;
        StartCoroutine(Co_HideTransit(color, duration));
    }

    public void ShowAndHide(Color color, float duration)
    {
        StartCoroutine(Co_ShowAndHideTransit(color, duration));
    }

    IEnumerator Co_ShowAndHideTransit(Color color, float duration)
    {
        yield return Co_ShowTransit(color, duration / 2);
        yield return Co_HideTransit(color, duration / 2);
    }

    IEnumerator Co_ShowTransit(Color color, float duration)
    {
        M_TransitState = TransitState.Trans;
        TransitPanel.raycastTarget = true;
        float tick = 0;
        while (tick < duration)
        {
            TransitPanel.color = new Color(color.r, color.g, color.b, tick / duration);
            tick += Time.deltaTime;
            yield return null;
        }

        M_TransitState = TransitState.Show;
        yield return null;
    }

    IEnumerator Co_HideTransit(Color color, float duration)
    {
        M_TransitState = TransitState.Trans;
        float tick = 0;
        while (tick < duration)
        {
            TransitPanel.color = new Color(color.r, color.g, color.b, 1 - tick / duration);
            tick += Time.deltaTime;
            yield return null;
        }

        TransitPanel.raycastTarget = false;
        M_TransitState = TransitState.Hide;
        yield return null;
    }

    private void ShowShutTransit(Color color, float duration)
    {
        StartCoroutine(Co_ShowShutTransit(color, duration));
    }

    IEnumerator Co_ShowShutTransit(Color color, float duration)
    {
        UpperTransitPanel.color = color;
        BottomTransitPanel.color = color;
        iTween.MoveTo(UpperTransitPanel.gameObject, UpperTransitShutPos.position, 0.3f);
        iTween.MoveTo(BottomTransitPanel.gameObject, BottomTransitShutPos.position, 0.3f);
        yield return new WaitForSeconds(duration + 0.3f);
        iTween.MoveTo(UpperTransitPanel.gameObject, UpperTransitHidePos.position, 0.3f);
        iTween.MoveTo(BottomTransitPanel.gameObject, BottomTransitHidePos.position, 0.3f);
        yield return new WaitForSeconds(0.3f);
    }

    public void ShowBlackShutTransit(float duration = 1f)
    {
        ShowShutTransit(Color.black, duration);
    }
}