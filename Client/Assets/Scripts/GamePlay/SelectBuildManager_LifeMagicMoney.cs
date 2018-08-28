using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口 - 选择卡组部分
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private GameObject MoneyBar;
    [SerializeField] private GameObject LifeBar;
    [SerializeField] private GameObject MagicBar;

    [SerializeField] private Slider MoneySlider;
    [SerializeField] private Slider LifeSlider;
    [SerializeField] private Slider MagicSlider;

    [SerializeField] private Text MyMoneyText;
    [SerializeField] private Text MyLifeText;
    [SerializeField] private Text MyMagicText;

    [SerializeField] private Text TotalMoneyText;
    [SerializeField] private Text MaxLifeText;
    [SerializeField] private Text MaxMagicText;

    [SerializeField] private Transform MyMoneyTextMinPos;
    [SerializeField] private Transform MyMoneyTextMaxPos;

    [SerializeField] private Transform MyLifeTextMinPos;
    [SerializeField] private Transform MyLifeTextMaxPos;

    [SerializeField] private Transform MyMagicTextMinPos;
    [SerializeField] private Transform MyMagicTextMaxPos;

    private void ShowSliders()
    {
        MoneyBar.SetActive(true);
        LifeBar.SetActive(true);
        MagicBar.SetActive(true);
    }

    private void HideSliders()
    {
        MoneyBar.SetActive(false);
        LifeBar.SetActive(false);
        MagicBar.SetActive(false);
    }

    private void InitializeSliders()
    {
        MoneySlider.value = (float) GamePlaySettings.PlayerDefaultMoney / GamePlaySettings.PlayerDefaultMaxMoney;
        LifeSlider.value = (float) GamePlaySettings.PlayerDefaultLife / GamePlaySettings.PlayerDefaultLifeMax;
        MagicSlider.value = (float) GamePlaySettings.PlayerDefaultMagic / GamePlaySettings.PlayerDefaultMagicMax;

        TotalMoneyText.text = GamePlaySettings.PlayerDefaultMaxMoney.ToString();
        MaxLifeText.text = GamePlaySettings.PlayerDefaultLifeMax.ToString();
        MaxMagicText.text = GamePlaySettings.PlayerDefaultMagicMax.ToString();

        MoneySlider.onValueChanged.AddListener(OnMoneySliderValueChange);
        LifeSlider.onValueChanged.AddListener(OnLifeSliderValueChange);
        MagicSlider.onValueChanged.AddListener(OnMagicSliderValueChange);
    }

    private void RefreshMoneyLifeMagic()
    {
        MoneySlider.value = (float) (GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney()) / GamePlaySettings.PlayerDefaultMaxMoney;
        OnMoneySliderValueChange(MoneySlider.value);
        LifeSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Life) / GamePlaySettings.PlayerDefaultLifeMax;
        OnLifeSliderValueChange(LifeSlider.value);
        MagicSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Magic) / GamePlaySettings.PlayerDefaultMagicMax;
        OnMagicSliderValueChange(MagicSlider.value);
    }

    private void OnMoneySliderValueChange(float value)
    {
        MyMoneyText.text = (GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney()).ToString();
        MyMoneyText.rectTransform.localPosition = Vector3.Lerp(MyMoneyTextMinPos.localPosition, MyMoneyTextMaxPos.localPosition, value);
    }


    private float noBudgetNoticeInterval = 3f;
    private float lastNoBudgetNoticeTime;
    private void OnLifeSliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.CardConsumeMoney - CurrentEditBuildButton.BuildInfo.MagicConsumeMoney + GamePlaySettings.PlayerDefaultLifeMin * GamePlaySettings.LifeToMoney) / GamePlaySettings.LifeToMoney) / GamePlaySettings.PlayerDefaultLifeMax;
        float minValue = (float) GamePlaySettings.PlayerDefaultLifeMin / GamePlaySettings.PlayerDefaultLifeMax;
        if (value > maxValueNow)
        {
            LifeSlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter("预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }
            return;
        }
        else if (value < minValue)
        {
            LifeSlider.value = minValue;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter("生命过低可不安全哦~", 0f, 1f);
            }
            return;
        }

        CurrentEditBuildButton.BuildInfo.Life = Mathf.RoundToInt(value * GamePlaySettings.PlayerDefaultLifeMax);
        CurrentEditBuildButton.BuildInfo.LifeConsumeMoney = (CurrentEditBuildButton.BuildInfo.Life - GamePlaySettings.PlayerDefaultLifeMin) * GamePlaySettings.LifeToMoney;
        MyLifeText.text = CurrentEditBuildButton.BuildInfo.Life.ToString();
        MyLifeText.rectTransform.localPosition = Vector3.Lerp(MyLifeTextMinPos.localPosition, MyLifeTextMaxPos.localPosition, value);

        MoneySlider.value = (float) (GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney()) / GamePlaySettings.PlayerDefaultMaxMoney;
    }

    private void OnMagicSliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.CardConsumeMoney - CurrentEditBuildButton.BuildInfo.LifeConsumeMoney) / GamePlaySettings.MagicToMoney) / GamePlaySettings.PlayerDefaultMagicMax;
        float minValue = 0;
        if (value > maxValueNow)
        {
            MagicSlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter("预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }
            return;
        }
        else if (value < minValue)
        {
            MagicSlider.value = minValue;
            return;
        }

        CurrentEditBuildButton.BuildInfo.Magic = Mathf.RoundToInt(value * GamePlaySettings.PlayerDefaultMagicMax);
        CurrentEditBuildButton.BuildInfo.MagicConsumeMoney = CurrentEditBuildButton.BuildInfo.Magic * GamePlaySettings.MagicToMoney;
        MyMagicText.text = CurrentEditBuildButton.BuildInfo.Magic.ToString();
        MyMagicText.rectTransform.localPosition = Vector3.Lerp(MyMagicTextMinPos.localPosition, MyMagicTextMaxPos.localPosition, value);

        MoneySlider.value = (float) (GamePlaySettings.PlayerDefaultMaxMoney - CurrentEditBuildButton.BuildInfo.GetBuildConsumeMoney()) / GamePlaySettings.PlayerDefaultMaxMoney;
    }
}