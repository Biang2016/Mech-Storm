using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口 - 选择卡组部分
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private GameObject CoinBar;
    [SerializeField] private GameObject LifeBar;
    [SerializeField] private GameObject EnergyBar;
    [SerializeField] private GameObject CardNumberBar;

    [SerializeField] private Slider CoinSlider;
    [SerializeField] private Slider LifeSlider;
    [SerializeField] private Slider EnergySlider;
    [SerializeField] private Slider CardNumberSlider;

    [SerializeField] private Text MyCoinText;
    [SerializeField] private Text MyLifeText;
    [SerializeField] private Text MyEnergyText;
    [SerializeField] private Text CardNumberText;

    [SerializeField] private Text AddCardNumberCoinText;
    [SerializeField] private Text DecCardNumberCoinText;
    [SerializeField] private Image AddCardNumberCoinImage;
    [SerializeField] private Image DecCardNumberCoinImage;
    [SerializeField] private Animator AddCardNumberCoinAnim;
    [SerializeField] private Animator DecCardNumberCoinAnim;

    [SerializeField] private Text TotalCoinText;
    [SerializeField] private Text MaxLifeText;
    [SerializeField] private Text MaxEnergyText;
    [SerializeField] private Text MaxCardNumberText;

    [SerializeField] private Transform MyCoinTextMinPos;
    [SerializeField] private Transform MyCoinTextMaxPos;

    [SerializeField] private Transform MyLifeTextMinPos;
    [SerializeField] private Transform MyLifeTextMaxPos;

    [SerializeField] private Transform MyEnergyTextMinPos;
    [SerializeField] private Transform MyEnergyTextMaxPos;

    [SerializeField] private Transform MyCardNumberTextMinPos;
    [SerializeField] private Transform MyCardNumberTextMaxPos;

    private void ShowSliders()
    {
        CoinBar.SetActive(true);
        LifeBar.SetActive(true);
        EnergyBar.SetActive(true);
        CardNumberBar.SetActive(true);
    }

    private void HideSliders()
    {
        CoinBar.SetActive(false);
        LifeBar.SetActive(false);
        EnergyBar.SetActive(false);
        CardNumberBar.SetActive(false);
    }

    public void InitializeSliders()
    {
        CoinSlider.onValueChanged.RemoveAllListeners();
        LifeSlider.onValueChanged.RemoveAllListeners();
        EnergySlider.onValueChanged.RemoveAllListeners();
        CardNumberSlider.onValueChanged.RemoveAllListeners();

        CoinSlider.value = (float) GamePlaySettings.DefaultCoin / GamePlaySettings.DefaultMaxCoin;
        LifeSlider.value = (float) GamePlaySettings.DefaultLife / GamePlaySettings.DefaultLifeMax;
        EnergySlider.value = (float) GamePlaySettings.DefaultEnergy / GamePlaySettings.DefaultEnergyMax;
        CardNumberSlider.value = GamePlaySettings.DefaultDrawCardNum;

        TotalCoinText.text = GamePlaySettings.DefaultMaxCoin.ToString();
        MaxLifeText.text = GamePlaySettings.DefaultLifeMax.ToString();
        MaxEnergyText.text = GamePlaySettings.DefaultEnergyMax.ToString();
        MaxCardNumberText.text = GamePlaySettings.MaxDrawCardNum.ToString();

        LanguageManager.Instance.RegisterTextFontBinding(new List<Text>
        {
            TotalCoinText,
            MaxLifeText,
            MaxEnergyText,
            MaxCardNumberText
        });

        CoinSlider.onValueChanged.AddListener(OnCoinSliderValueChange);
        LifeSlider.onValueChanged.AddListener(OnLifeSliderValueChange);
        EnergySlider.onValueChanged.AddListener(OnEnergySliderValueChange);
        CardNumberSlider.onValueChanged.AddListener(OnCardNumSliderValueChange);

        RefreshDrawCardCoinText(GamePlaySettings.DefaultDrawCardNum);
    }

    private void RefreshDrawCardCoinText(int drawCardNum)
    {
        if (drawCardNum == GamePlaySettings.MaxDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = false;
            AddCardNumberCoinText.text = "";
            DecCardNumberCoinImage.enabled = true;
            DecCardNumberCoinText.text = "+" + (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum - 1]).ToString();
        }
        else if (drawCardNum < GamePlaySettings.MaxDrawCardNum && drawCardNum > GamePlaySettings.MinDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = true;
            AddCardNumberCoinText.text = (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum + 1]).ToString();
            DecCardNumberCoinImage.enabled = true;
            DecCardNumberCoinText.text = "+" + (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum - 1]).ToString();
        }
        else if (drawCardNum == GamePlaySettings.MinDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = true;
            AddCardNumberCoinText.text = (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum + 1]).ToString();
            DecCardNumberCoinImage.enabled = false;
            DecCardNumberCoinText.text = "";
        }
    }

    private void RefreshCoinLifeEnergy()
    {
        CurrentEditBuildButton.BuildInfo.GamePlaySettings = GamePlaySettings;
        CoinSlider.value = (float) (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) / GamePlaySettings.DefaultMaxCoin;
        OnCoinSliderValueChange(CoinSlider.value);
        LifeSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Life) / GamePlaySettings.DefaultLifeMax;
        OnLifeSliderValueChange(LifeSlider.value);
        EnergySlider.value = (float) (CurrentEditBuildButton.BuildInfo.Energy) / GamePlaySettings.DefaultEnergyMax;
        OnEnergySliderValueChange(EnergySlider.value);
    }

    private void RefreshCardNum()
    {
        CardNumberSlider.value = CurrentEditBuildButton.BuildInfo.DrawCardNum;
        OnCardNumSliderValueChange(CurrentEditBuildButton.BuildInfo.DrawCardNum);
    }

    private void OnCoinSliderValueChange(float value)
    {
        MyCoinText.text = (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin).ToString();
        MyCoinText.rectTransform.localPosition = Vector3.Lerp(MyCoinTextMinPos.localPosition, MyCoinTextMaxPos.localPosition, value);
    }

    private float noBudgetNoticeInterval = 3f;
    private float lastNoBudgetNoticeTime;

    private void OnLifeSliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin - CurrentEditBuildButton.BuildInfo.DrawCardNumConsumeCoin + GamePlaySettings.DefaultLifeMin * GamePlaySettings.LifeToCoin) /
                                     GamePlaySettings.LifeToCoin) / GamePlaySettings.DefaultLifeMax;
        float minValue = (float) GamePlaySettings.DefaultLifeMin / GamePlaySettings.DefaultLifeMax;
        if (value > maxValueNow)
        {
            LifeSlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_BudgetLimitedChooseFewerCards"), 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            LifeSlider.value = minValue;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerLifeMagicCoinCardnum_UnsafeIfLifeLow"), 0f, 1f);
            }

            return;
        }

        CurrentEditBuildButton.BuildInfo.Life = Mathf.RoundToInt(value * GamePlaySettings.DefaultLifeMax);
        MyLifeText.text = CurrentEditBuildButton.BuildInfo.Life.ToString();
        MyLifeText.rectTransform.localPosition = Vector3.Lerp(MyLifeTextMinPos.localPosition, MyLifeTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) / GamePlaySettings.DefaultMaxCoin;
    }

    private void OnEnergySliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin - CurrentEditBuildButton.BuildInfo.DrawCardNumConsumeCoin) / GamePlaySettings.EnergyToCoin) / GamePlaySettings.DefaultEnergyMax;
        float minValue = 0;
        if (value > maxValueNow)
        {
            EnergySlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_BudgetLimitedChooseFewerCards"), 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            EnergySlider.value = minValue;
            return;
        }

        CurrentEditBuildButton.BuildInfo.Energy = Mathf.RoundToInt(value * GamePlaySettings.DefaultEnergyMax);
        MyEnergyText.text = CurrentEditBuildButton.BuildInfo.Energy.ToString();
        MyEnergyText.rectTransform.localPosition = Vector3.Lerp(MyEnergyTextMinPos.localPosition, MyEnergyTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) / GamePlaySettings.DefaultMaxCoin;
    }

    private void OnCardNumSliderValueChange(float value)
    {
        int drawCardNum = Mathf.RoundToInt(value);
        if (drawCardNum == CurrentEditBuildButton.BuildInfo.DrawCardNum)
        {
            CardNumberText.text = drawCardNum.ToString();
            CardNumberText.rectTransform.localPosition = Vector3.Lerp(MyCardNumberTextMinPos.localPosition, MyCardNumberTextMaxPos.localPosition, (float) value / (float) GamePlaySettings.MaxDrawCardNum);
            CoinSlider.value = (float) (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) / GamePlaySettings.DefaultMaxCoin;
            return;
        }

        int leftCoin = GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin;
        int minDrawCardNum = GamePlaySettings.MinDrawCardNum;
        int maxDrawCardNum = 0;
        for (int i = GamePlaySettings.MinDrawCardNum; i <= GamePlaySettings.MaxDrawCardNum; i++)
        {
            if (GamePlaySettings.DrawCardNumToCoin[i] - GamePlaySettings.DrawCardNumToCoin[GamePlaySettings.MinDrawCardNum] <= leftCoin)
            {
                maxDrawCardNum = i;
            }
        }

        if (drawCardNum > maxDrawCardNum)
        {
            CardNumberSlider.value = maxDrawCardNum;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_BudgetLimitedChooseFewerCards"), 0f, 1f);
            }

            return;
        }
        else if (drawCardNum < minDrawCardNum)
        {
            CardNumberSlider.value = minDrawCardNum;
            return;
        }

        CurrentEditBuildButton.BuildInfo.DrawCardNum = drawCardNum;
        CardNumberText.text = drawCardNum.ToString();
        CardNumberText.rectTransform.localPosition = Vector3.Lerp(MyCardNumberTextMinPos.localPosition, MyCardNumberTextMaxPos.localPosition, (float) value / (float) GamePlaySettings.MaxDrawCardNum);

        CoinSlider.value = (float) (GamePlaySettings.DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.BuildConsumeCoin) / GamePlaySettings.DefaultMaxCoin;

        AddCardNumberCoinAnim.SetTrigger("Jump");
        DecCardNumberCoinAnim.SetTrigger("Jump");
        AudioManager.Instance.SoundPlay("sfx/OnMoneyChange");
        RefreshDrawCardCoinText(drawCardNum);
    }
}