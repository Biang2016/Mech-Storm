using JetBrains.Annotations;
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

    private void InitializeSliders()
    {
        CoinSlider.value = (float) GamePlaySettings.PlayerDefaultCoin / GamePlaySettings.PlayerDefaultMaxCoin;
        LifeSlider.value = (float) GamePlaySettings.PlayerDefaultLife / GamePlaySettings.PlayerDefaultLifeMax;
        EnergySlider.value = (float) GamePlaySettings.PlayerDefaultEnergy / GamePlaySettings.PlayerDefaultEnergyMax;
        CardNumberSlider.value = GamePlaySettings.PlayerDefaultDrawCardNum;

        TotalCoinText.text = GamePlaySettings.PlayerDefaultMaxCoin.ToString();
        MaxLifeText.text = GamePlaySettings.PlayerDefaultLifeMax.ToString();
        MaxEnergyText.text = GamePlaySettings.PlayerDefaultEnergyMax.ToString();
        MaxCardNumberText.text = GamePlaySettings.PlayerMaxDrawCardNum.ToString();

        CoinSlider.onValueChanged.AddListener(OnCoinSliderValueChange);
        LifeSlider.onValueChanged.AddListener(OnLifeSliderValueChange);
        EnergySlider.onValueChanged.AddListener(OnEnergySliderValueChange);
        CardNumberSlider.onValueChanged.AddListener(OnCardNumSliderValueChange);
    }

    private void RefreshCoinLifeEnergy()
    {
        CoinSlider.value = (float) (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / GamePlaySettings.PlayerDefaultMaxCoin;
        OnCoinSliderValueChange(CoinSlider.value);
        LifeSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Life) / GamePlaySettings.PlayerDefaultLifeMax;
        OnLifeSliderValueChange(LifeSlider.value);
        EnergySlider.value = (float) (CurrentEditBuildButton.BuildInfo.Energy) / GamePlaySettings.PlayerDefaultEnergyMax;
        OnEnergySliderValueChange(EnergySlider.value);
        CardNumberSlider.value = CurrentEditBuildButton.BuildInfo.DrawCardNum;
        OnCardNumSliderValueChange(CardNumberSlider.value);
    }

    private void OnCoinSliderValueChange(float value)
    {
        MyCoinText.text = (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()).ToString();
        MyCoinText.rectTransform.localPosition = Vector3.Lerp(MyCoinTextMinPos.localPosition, MyCoinTextMaxPos.localPosition, value);
    }


    private float noBudgetNoticeInterval = 3f;
    private float lastNoBudgetNoticeTime;

    private void OnLifeSliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin + GamePlaySettings.PlayerDefaultLifeMin * GamePlaySettings.LifeToCoin) / GamePlaySettings.LifeToCoin) / GamePlaySettings.PlayerDefaultLifeMax;
        float minValue = (float) GamePlaySettings.PlayerDefaultLifeMin / GamePlaySettings.PlayerDefaultLifeMax;
        if (value > maxValueNow)
        {
            LifeSlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            LifeSlider.value = minValue;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "It's unsafe if your life is too low." : "生命过低可不安全哦~", 0f, 1f);
            }

            return;
        }

        CurrentEditBuildButton.BuildInfo.Life = Mathf.RoundToInt(value * GamePlaySettings.PlayerDefaultLifeMax);
        CurrentEditBuildButton.BuildInfo.LifeConsumeCoin = (CurrentEditBuildButton.BuildInfo.Life - GamePlaySettings.PlayerDefaultLifeMin) * GamePlaySettings.LifeToCoin;
        MyLifeText.text = CurrentEditBuildButton.BuildInfo.Life.ToString();
        MyLifeText.rectTransform.localPosition = Vector3.Lerp(MyLifeTextMinPos.localPosition, MyLifeTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / GamePlaySettings.PlayerDefaultMaxCoin;
    }

    private void OnEnergySliderValueChange(float value)
    {
        float maxValueNow = (float) ((GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin) / GamePlaySettings.EnergyToCoin) / GamePlaySettings.PlayerDefaultEnergyMax;
        float minValue = 0;
        if (value > maxValueNow)
        {
            EnergySlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            EnergySlider.value = minValue;
            return;
        }

        CurrentEditBuildButton.BuildInfo.Energy = Mathf.RoundToInt(value * GamePlaySettings.PlayerDefaultEnergyMax);
        CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin = CurrentEditBuildButton.BuildInfo.Energy * GamePlaySettings.EnergyToCoin;
        MyEnergyText.text = CurrentEditBuildButton.BuildInfo.Energy.ToString();
        MyEnergyText.rectTransform.localPosition = Vector3.Lerp(MyEnergyTextMinPos.localPosition, MyEnergyTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / GamePlaySettings.PlayerDefaultMaxCoin;
    }

    private void OnCardNumSliderValueChange(float value)
    {
        int leftCoin = GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin;
        int minDrawCardNum = GamePlaySettings.PlayerMinDrawCardNum;
        int maxDrawCardNum = 0;
        for (int i = GamePlaySettings.PlayerMinDrawCardNum; i <= GamePlaySettings.PlayerMaxDrawCardNum; i++)
        {
            if (GamePlaySettings.DrawCardNumToCoin[i] - GamePlaySettings.DrawCardNumToCoin[GamePlaySettings.PlayerDefaultDrawCardNum] <= leftCoin)
            {
                maxDrawCardNum = i;
            }
        }

        int drawCardNum = Mathf.RoundToInt(value);

        if (drawCardNum > maxDrawCardNum)
        {
            CardNumberSlider.value = maxDrawCardNum;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
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
        CardNumberText.rectTransform.localPosition = Vector3.Lerp(MyCardNumberTextMinPos.localPosition, MyCardNumberTextMaxPos.localPosition, (float) value / (float) GamePlaySettings.PlayerMaxDrawCardNum);

        CoinSlider.value = (float) (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / GamePlaySettings.PlayerDefaultMaxCoin;
    }
}