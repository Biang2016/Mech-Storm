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
    
    bool IsServerBuild = false;

    private int DefaultCoin
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultCoin : GamePlaySettings.PlayerDefaultCoin; }
    }

    private int DefaultMaxCoin
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultMaxCoin : GamePlaySettings.PlayerDefaultMaxCoin; }
    }

    private int DefaultLife
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultLife : GamePlaySettings.PlayerDefaultLife; }
    }

    private int DefaultLifeMax
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultLifeMax : GamePlaySettings.PlayerDefaultLifeMax; }
    }

    private int DefaultLifeMin
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultLifeMin : GamePlaySettings.PlayerDefaultLifeMin; }
    }

    private int DefaultEnergy
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultEnergy : GamePlaySettings.PlayerDefaultEnergy; }
    }

    private int DefaultEnergyMax
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultEnergyMax : GamePlaySettings.PlayerDefaultEnergyMax; }
    }

    private int DefaultDrawCardNum
    {
        get { return IsServerBuild ? GamePlaySettings.ServerDefaultDrawCardNum : GamePlaySettings.PlayerDefaultDrawCardNum; }
    }

    private int MinDrawCardNum
    {
        get { return IsServerBuild ? GamePlaySettings.ServerMinDrawCardNum : GamePlaySettings.PlayerMinDrawCardNum; }
    }

    private int MaxDrawCardNum
    {
        get { return IsServerBuild ? GamePlaySettings.ServerMaxDrawCardNum : GamePlaySettings.PlayerMaxDrawCardNum; }
    }

    private void InitializeSliders(bool isServerBuild = false)
    {
        IsServerBuild = isServerBuild;

        CoinSlider.value = (float) DefaultCoin / DefaultMaxCoin;
        LifeSlider.value = (float) DefaultLife / DefaultLifeMax;
        EnergySlider.value = (float) DefaultEnergy / DefaultEnergyMax;
        CardNumberSlider.value = DefaultDrawCardNum;


        TotalCoinText.text = DefaultMaxCoin.ToString();
        MaxLifeText.text = DefaultLifeMax.ToString();
        MaxEnergyText.text = DefaultEnergyMax.ToString();
        MaxCardNumberText.text = MaxDrawCardNum.ToString();

        TotalCoinText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        MaxLifeText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        MaxEnergyText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;
        MaxCardNumberText.font = GameManager.Instance.IsEnglish ? GameManager.Instance.EnglishFont : GameManager.Instance.ChineseFont;

        CoinSlider.onValueChanged.AddListener(OnCoinSliderValueChange);
        LifeSlider.onValueChanged.AddListener(OnLifeSliderValueChange);
        EnergySlider.onValueChanged.AddListener(OnEnergySliderValueChange);
        CardNumberSlider.onValueChanged.AddListener(OnCardNumSliderValueChange);

        RefreshDrawCardCoinText(DefaultDrawCardNum);
    }

    private void RefreshDrawCardCoinText(int drawCardNum)
    {
        if (drawCardNum == MaxDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = false;
            AddCardNumberCoinText.text = "";
            DecCardNumberCoinImage.enabled = true;
            DecCardNumberCoinText.text = "+" + (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum - 1]).ToString();
        }
        else if (drawCardNum < MaxDrawCardNum && drawCardNum > MinDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = true;
            AddCardNumberCoinText.text = (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum + 1]).ToString();
            DecCardNumberCoinImage.enabled = true;
            DecCardNumberCoinText.text = "+" + (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum - 1]).ToString();
        }
        else if (drawCardNum == MinDrawCardNum)
        {
            AddCardNumberCoinImage.enabled = true;
            AddCardNumberCoinText.text = (GamePlaySettings.DrawCardNumToCoin[drawCardNum] - GamePlaySettings.DrawCardNumToCoin[drawCardNum + 1]).ToString();
            DecCardNumberCoinImage.enabled = false;
            DecCardNumberCoinText.text = "";
        }
    }

    private void RefreshCoinLifeEnergy()
    {
        CoinSlider.value = (float) (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / DefaultMaxCoin;
        OnCoinSliderValueChange(CoinSlider.value);
        LifeSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Life) / DefaultLifeMax;
        OnLifeSliderValueChange(LifeSlider.value);
        EnergySlider.value = (float) (CurrentEditBuildButton.BuildInfo.Energy) / DefaultEnergyMax;
        OnEnergySliderValueChange(EnergySlider.value);
    }

    private void RefreshCardNum()
    {
        CardNumberSlider.value = CurrentEditBuildButton.BuildInfo.DrawCardNum;
        OnCardNumSliderValueChange(CurrentEditBuildButton.BuildInfo.DrawCardNum);
    }

    private void OnCoinSliderValueChange(float value)
    {
        MyCoinText.text = (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()).ToString();
        MyCoinText.rectTransform.localPosition = Vector3.Lerp(MyCoinTextMinPos.localPosition, MyCoinTextMaxPos.localPosition, value);
    }


    private float noBudgetNoticeInterval = 3f;
    private float lastNoBudgetNoticeTime;

    private void OnLifeSliderValueChange(float value)
    {
        float maxValueNow = (float) ((DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin - CurrentEditBuildButton.BuildInfo.DrawCardNumConsumeCoin + DefaultLifeMin * GamePlaySettings.LifeToCoin) / GamePlaySettings.LifeToCoin) / DefaultLifeMax;
        float minValue = (float) DefaultLifeMin / DefaultLifeMax;
        if (value > maxValueNow)
        {
            LifeSlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            LifeSlider.value = minValue;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "It's unsafe if your life is too low." : "生命过低可不安全哦~", 0f, 1f);
            }

            return;
        }

        CurrentEditBuildButton.BuildInfo.Life = Mathf.RoundToInt(value * DefaultLifeMax);
        MyLifeText.text = CurrentEditBuildButton.BuildInfo.Life.ToString();
        MyLifeText.rectTransform.localPosition = Vector3.Lerp(MyLifeTextMinPos.localPosition, MyLifeTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / DefaultMaxCoin;
    }

    private void OnEnergySliderValueChange(float value)
    {
        float maxValueNow = (float) ((DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin - CurrentEditBuildButton.BuildInfo.DrawCardNumConsumeCoin) / GamePlaySettings.EnergyToCoin) / DefaultEnergyMax;
        float minValue = 0;
        if (value > maxValueNow)
        {
            EnergySlider.value = maxValueNow;
            if (Time.time - lastNoBudgetNoticeTime > noBudgetNoticeInterval)
            {
                lastNoBudgetNoticeTime = Time.time;
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
            }

            return;
        }
        else if (value < minValue)
        {
            EnergySlider.value = minValue;
            return;
        }

        CurrentEditBuildButton.BuildInfo.Energy = Mathf.RoundToInt(value * DefaultEnergyMax);
        MyEnergyText.text = CurrentEditBuildButton.BuildInfo.Energy.ToString();
        MyEnergyText.rectTransform.localPosition = Vector3.Lerp(MyEnergyTextMinPos.localPosition, MyEnergyTextMaxPos.localPosition, value);

        CoinSlider.value = (float) (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / DefaultMaxCoin;
    }

    private void OnCardNumSliderValueChange(float value)
    {
        int drawCardNum = Mathf.RoundToInt(value);
        if (drawCardNum == CurrentEditBuildButton.BuildInfo.DrawCardNum)
        {
            CardNumberText.text = drawCardNum.ToString();
            CardNumberText.rectTransform.localPosition = Vector3.Lerp(MyCardNumberTextMinPos.localPosition, MyCardNumberTextMaxPos.localPosition, (float) value / (float) MaxDrawCardNum);
            CoinSlider.value = (float) (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / DefaultMaxCoin;
            return;
        }

        int leftCoin = DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.CardConsumeCoin - CurrentEditBuildButton.BuildInfo.LifeConsumeCoin - CurrentEditBuildButton.BuildInfo.EnergyConsumeCoin;
        int minDrawCardNum = MinDrawCardNum;
        int maxDrawCardNum = 0;
        for (int i = MinDrawCardNum; i <= MaxDrawCardNum; i++)
        {
            if (GamePlaySettings.DrawCardNumToCoin[i] - GamePlaySettings.DrawCardNumToCoin[MinDrawCardNum] <= leftCoin)
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
                NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Bugget is Limited. Please consider choosing fewer cards." : "预算不足,考虑少选一些卡牌哦~", 0f, 1f);
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
        CardNumberText.rectTransform.localPosition = Vector3.Lerp(MyCardNumberTextMinPos.localPosition, MyCardNumberTextMaxPos.localPosition, (float) value / (float) MaxDrawCardNum);

        CoinSlider.value = (float) (DefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / DefaultMaxCoin;

        AddCardNumberCoinAnim.SetTrigger("Jump");
        DecCardNumberCoinAnim.SetTrigger("Jump");
        AudioManager.Instance.SoundPlay("sfx/OnMoneyChange");
        RefreshDrawCardCoinText(drawCardNum);
    }
}