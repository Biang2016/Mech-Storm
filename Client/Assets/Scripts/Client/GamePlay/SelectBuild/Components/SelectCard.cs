using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class SelectCard : PoolObject
{
    public override void PoolRecycle()
    {
        Count = 0;
        base.PoolRecycle();
    }

    void Awake()
    {
    }

    [SerializeField] private Transform Tran_Metal;
    [SerializeField] private Transform Tran_Energy;
    [SerializeField] private Transform Tran_Metal_s;
    [SerializeField] private Transform Tran_Energy_s;
    [SerializeField] private Transform Tran_NOCOST;

    [SerializeField] private Text Text_Metal;
    [SerializeField] private Text Text_Energy;
    [SerializeField] private Text Text_Metal_s;
    [SerializeField] private Text Text_Energy_s;
    [SerializeField] private Text Text_NOCOST;

    [SerializeField] private Text Text_MetalBG;
    [SerializeField] private Text Text_EnergyBG;
    [SerializeField] private Text Text_Metal_sBG;
    [SerializeField] private Text Text_Energy_sBG;
    [SerializeField] private Text Text_NOCOSTBG;

    [SerializeField] private Text Text_Count;
    public Button CardButton;
    [SerializeField] private Text Text_CardName;
    [SerializeField] private Image CardImage;

    [SerializeField] private StarsGroup StarsGroup;

    private int count;

    public int Count
    {
        get { return count; }

        set
        {
            count = value;
            Text_Count.text = "×" + count.ToString();
        }
    }

    private int metal;

    public int Metal
    {
        get { return metal; }

        set
        {
            metal = value;
            RefreshText();
        }
    }

    private int energy;

    public int Energy
    {
        get { return energy; }

        set
        {
            energy = value;
            RefreshText();
        }
    }

    public CardInfo_Base CardInfo;

    public void Initiate(int count, CardInfo_Base cardInfo, SelectCardOnMouseEnterHandler enterHandler, SelectCardOnMouseLeaveHandler leaveHandler, Color color)
    {
        Count = count;
        Metal = cardInfo.BaseInfo.Metal;
        Energy = cardInfo.BaseInfo.Energy;
        Text_CardName.text = cardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        CardInfo = cardInfo;
        OnMouseEnterHandler = enterHandler;
        OnMouseLeaveHandler = leaveHandler;
        CardButton.image.color = color;
        CardButton.onClick.RemoveAllListeners();
        ClientUtils.ChangeImagePicture(CardImage, CardInfo.BaseInfo.PictureID);
        StarsGroup.SetStarNumber(CardInfo.UpgradeInfo.CardLevel, CardInfo.UpgradeInfo.CardLevelMax);
    }

    public void RefreshTextLanguage()
    {
        Text_CardName.text = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
    }

    private void RefreshText()
    {
        if (Metal != 0 && Energy != 0)
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(true);
            Tran_Energy_s.gameObject.SetActive(true);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Metal_s.text = Metal.ToString();
            Text_Energy_s.text = Energy.ToString();
            Text_Metal_sBG.text = Metal.ToString();
            Text_Energy_sBG.text = Energy.ToString();
        }
        else if (Metal == 0 && Energy != 0)
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(true);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Energy.text = Energy.ToString();
            Text_EnergyBG.text = Energy.ToString();
        }
        else if (Metal != 0 && Energy == 0)
        {
            Tran_Metal.gameObject.SetActive(true);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Metal.text = Metal.ToString();
            Text_MetalBG.text = Metal.ToString();
        }
        else
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(true);

            Text_NOCOST.text = "-";
            Text_NOCOSTBG.text = "-";
        }
    }

    public delegate void SelectCardOnMouseEnterHandler(SelectCard selectCard);

    public SelectCardOnMouseEnterHandler OnMouseEnterHandler;

    public delegate void SelectCardOnMouseLeaveHandler(SelectCard selectCard);

    public SelectCardOnMouseLeaveHandler OnMouseLeaveHandler;

    public void MouseEnter()
    {
        OnMouseEnterHandler(this);
    }

    public void MouseLeave()
    {
        OnMouseLeaveHandler(this);
    }
}