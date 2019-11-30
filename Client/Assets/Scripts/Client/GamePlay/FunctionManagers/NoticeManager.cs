using System.Collections;
using TMPro;
using UnityEngine;

public class NoticeManager : MonoSingleton<NoticeManager>
{
    private NoticeManager()
    {
    }

    private void Awake()
    {
        InfoCanvas.enabled = true;
        InfoPanelTopAnimator.gameObject.SetActive(true);
        InfoPanelCenterAnimator.gameObject.SetActive(true);
    }

    #region InfoPanelTop

    [SerializeField] private Canvas InfoCanvas;
    [SerializeField] private Animator InfoPanelTopAnimator;
    [SerializeField] private TextMeshPro InfoTextTop;

    IEnumerator ShowInfoPanelTopCoroutine;

    public void ShowInfoPanelTop(string text, float delay, float last)
    {
        if (Instance == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (ShowInfoPanelTopCoroutine != null)
        {
            StopCoroutine(ShowInfoPanelTopCoroutine);
        }

        ShowInfoPanelTopCoroutine = Co_ShowInfoPanelTop(text, delay, last);
        StartCoroutine(ShowInfoPanelTopCoroutine);
    }

    IEnumerator Co_ShowInfoPanelTop(string text, float delay, float last)
    {
        yield return new WaitForSeconds(delay);
        InfoTextTop.text = text;
        if (InfoPanelTopAnimator.GetBool("isShow"))
        {
            InfoPanelTopAnimator.SetTrigger("Shut");
        }

        InfoPanelTopAnimator.SetBool("isShow", true);
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            InfoPanelTopAnimator.SetBool("isShow", false);
        }
        else
        {
            int dotCount = 0;
            while (true)
            {
                InfoTextTop.text += ".";
                yield return new WaitForSeconds(0.5f);
                dotCount++;
                if (dotCount == 3)
                {
                    dotCount = 0;
                    InfoTextTop.text = text;
                }
            }
        }
    }

    #endregion

    #region InfoPanelCenter

    [SerializeField] private Animator InfoPanelCenterAnimator;
    [SerializeField] private TextMeshPro InfoTextCenter;

    IEnumerator ShowInfoPanelCenterCoroutine;

    public void ShowInfoPanelCenter(string text, float delay, float last)
    {
        if (ShowInfoPanelCenterCoroutine != null)
        {
            StopCoroutine(ShowInfoPanelCenterCoroutine);
        }

        ShowInfoPanelCenterCoroutine = Co_ShowInfoPanelCenter(text, delay, last);
        StartCoroutine(ShowInfoPanelCenterCoroutine);
    }

    IEnumerator Co_ShowInfoPanelCenter(string text, float delay, float last)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.SoundPlay("sfx/NoticeInfo", 0.6f);
        InfoTextCenter.text = text;
        if (InfoPanelCenterAnimator.GetBool("isShow"))
        {
            InfoPanelCenterAnimator.SetTrigger("Shut");
        }

        InfoPanelCenterAnimator.SetBool("isShow", true);
        if (!float.IsPositiveInfinity(last))
        {
            yield return new WaitForSeconds(last);
            InfoPanelCenterAnimator.SetBool("isShow", false);
        }
        else
        {
            int dotCount = 0;
            while (true)
            {
                InfoTextCenter.text += ".";
                yield return new WaitForSeconds(0.5f);
                dotCount++;
                if (dotCount == 3)
                {
                    dotCount = 0;
                    InfoTextCenter.text = text;
                }
            }
        }
    }

    #endregion
}