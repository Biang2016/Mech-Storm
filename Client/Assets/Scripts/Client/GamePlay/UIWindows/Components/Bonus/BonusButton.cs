using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class BonusButton : PoolObject
{
    public override void PoolRecycle()
    {
        Reset();
        base.PoolRecycle();
    }

    public BonusGroup BonusGroup;

    [SerializeField] private Image BG_Image;
    [SerializeField] private Transform Container;
    [SerializeField] private Button Button;
    [SerializeField] private Image BorderSelected;
    [SerializeField] private Image Border;
    [SerializeField] private ParticleSystem Particle;

    public List<BonusItem_Base> M_BonusItems = new List<BonusItem_Base>();

    public void Reset()
    {
        foreach (BonusItem_Base bonusItem in M_BonusItems)
        {
            bonusItem.PoolRecycle();
        }

        M_BonusItems.Clear();
        SetSelected(false);
        BG_Image.gameObject.SetActive(true);
        BorderSelected.gameObject.SetActive(true);
        Border.gameObject.SetActive(true);
        card = null;
        IsSelected = false;
        Particle.gameObject.SetActive(false);
    }

    public void Initialize(BonusGroup bonusGroup)
    {
        Reset();

        BonusGroup = bonusGroup;
        if (BonusGroup.IsAlways)
        {
            Button.interactable = false;
            Button.enabled = false;
            Button.onClick.RemoveAllListeners();
        }
        else
        {
            Button.interactable = true;
            Button.enabled = true;
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(OnButtonClick);
        }

        foreach (Bonus bonus in BonusGroup.Bonuses)
        {
            BonusItem_Base bi = BonusItem_Base.InstantiateBonusItem(bonus, Container);
            M_BonusItems.Add(bi);
        }

        if (M_BonusItems.Count == 1 && M_BonusItems[0] is BigBonusItem)
        {
            BigBonusItem bigBonusItem = (BigBonusItem) M_BonusItems[0];
            if (bigBonusItem.CurrentCard != null)
            {
                card = bigBonusItem.CurrentCard;
                card.BeBrightColor();
                BG_Image.gameObject.SetActive(false);
                BorderSelected.gameObject.SetActive(false);
                Border.gameObject.SetActive(false);
            }
            else
            {
                int a = 0;
            }
        }

        Particle.gameObject.SetActive(false);
    }

    private CardBase card = null;

    public void OnButtonClick()
    {
        UIManager.Instance.GetBaseUIForm<BattleResultPanel>().SetBonusButtonSelected(this);
        AudioManager.Instance.SoundPlay("sfx/OnStoryButtonClick");
    }

    public void OnConfirmButtonClickDelegate()
    {
        UIManager.Instance.GetBaseUIForm<BattleResultPanel>().GetBonusBuildChangeInfo(BonusGroup);
        BonusGroupRequest request = new BonusGroupRequest(Client.Instance.Proxy.ClientID, BonusGroup);
        Client.Instance.Proxy.SendMessage(request);
        foreach (Bonus bonus in BonusGroup.Bonuses)
        {
            if (bonus is Bonus_UnlockCardByID b_UnlockCardByID)
            {
                StoryManager.Instance.JustGetNewCards.Add(b_UnlockCardByID.CardID);
            }
        }
    }

    private bool IsSelected = false;

    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        BorderSelected.enabled = isSelected;
        Particle.gameObject.SetActive(!card && isSelected);
    }

    public void OnHover()
    {
        if (card)
        {
            UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {card.CardInfo}, null);
        }

        if (IsSelected) return;
    }

    public void OnExit()
    {
        if (card)
        {
            UIManager.Instance.CloseUIForm<AffixPanel>();
        }

        if (IsSelected) return;
    }
}