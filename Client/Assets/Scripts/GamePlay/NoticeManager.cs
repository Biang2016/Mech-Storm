using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : MonoSingletion<NoticeManager>
{
    private NoticeManager()
    {
    }

    [SerializeField] private Animator InfoPanelAnimator;
    [SerializeField] private Text InfoText;

    IEnumerator ShowInfoPanelCoroutine;

    public void ShowInfoPanel(string text, float delay, float last)
    {
        if (ShowInfoPanelCoroutine != null)
        {
            StopCoroutine(ShowInfoPanelCoroutine);
        }

        ShowInfoPanelCoroutine = Co_ShowInfoPanel(text, delay, last);
        StartCoroutine(ShowInfoPanelCoroutine);
    }

    IEnumerator Co_ShowInfoPanel(string text, float delay, float last)
    {
        yield return new WaitForSeconds(delay);
        InfoText.text = text;
        if (InfoPanelAnimator.GetBool("isShow"))
        {
            InfoPanelAnimator.SetTrigger("Shut");
        }

        InfoPanelAnimator.SetBool("isShow", true);
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            InfoPanelAnimator.SetBool("isShow", false);
        }
        else
        {
            int dotCount = 0;
            while (true)
            {
                InfoText.text += ".";
                yield return new WaitForSeconds(0.5f);
                dotCount++;
                if (dotCount == 3)
                {
                    dotCount = 0;
                    InfoText.text = text;
                }
            }
        }
    }
}