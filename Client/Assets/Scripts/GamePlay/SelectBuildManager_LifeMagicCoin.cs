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

    [SerializeField] private Slider CoinSlider;
    [SerializeField] private Slider LifeSlider;
    [SerializeField] private Slider EnergySlider;

    [SerializeField] private Text MyCoinText;
    [SerializeField] private Text MyLifeText;
    [SerializeField] private Text MyEnergyText;

    [SerializeField] private Text TotalCoinText;
    [SerializeField] private Text MaxLifeText;
    [SerializeField] private Text MaxEnergyText;

    [SerializeField] private Transform MyCoinTextMinPos;
    [SerializeField] private Transform MyCoinTextMaxPos;

    [SerializeField] private Transform MyLifeTextMinPos;
    [SerializeField] private Transform MyLifeTextMaxPos;

    [SerializeField] private Transform MyEnergyTextMinPos;
    [SerializeField] private Transform MyEnergyTextMaxPos;

    private void ShowSliders()
    {
        CoinBar.SetActive(true);
        LifeBar.SetActive(true);
        EnergyBar.SetActive(true);
    }

    private void HideSliders()
    {
        CoinBar.SetActive(false);
        LifeBar.SetActive(false);
        EnergyBar.SetActive(false);
    }

    private void InitializeSliders()
    {
        CoinSlider.value = (float) GamePlaySettings.PlayerDefaultCoin / GamePlaySettings.PlayerDefaultMaxCoin;
        LifeSlider.value = (float) GamePlaySettings.PlayerDefaultLife / GamePlaySettings.PlayerDefaultLifeMax;
        EnergySlider.value = (float) GamePlaySettings.PlayerDefaultEnergy / GamePlaySettings.PlayerDefaultEnergyMax;

        TotalCoinText.text = GamePlaySettings.PlayerDefaultMaxCoin.ToString();
        MaxLifeText.text = GamePlaySettings.PlayerDefaultLifeMax.ToString();
        MaxEnergyText.text = GamePlaySettings.PlayerDefaultEnergyMax.ToString();

        CoinSlider.onValueChanged.AddListener(OnCoinSliderValueChange);
        LifeSlider.onValueChanged.AddListener(OnLifeSliderValueChange);
        EnergySlider.onValueChanged.AddListener(OnEnergySliderValueChange);
    }

    private void RefreshCoinLifeEnergy()
    {
        CoinSlider.value = (float) (GamePlaySettings.PlayerDefaultMaxCoin - CurrentEditBuildButton.BuildInfo.GetBuildConsumeCoin()) / GamePlaySettings.PlayerDefaultMaxCoin;
        OnCoinSliderValueChange(CoinSlider.value);
        LifeSlider.value = (float) (CurrentEditBuildButton.BuildInfo.Life) / GamePlaySettings.PlayerDefaultLifeMax;
        OnLifeSliderValueChange(LifeSlider.value);
        EnergySlider.value = (float) (CurrentEditBuildButton.BuildInfo.Energy) / GamePlaySettings.PlayerDefaultEnergyMax;
        OnEnergySliderValueChange(EnergySlider.value);
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
                NoticeManager.Instance.ShowInfoPanelCenter("预算不足,考虑少选一些卡牌哦~", 0f, 1f);
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
}