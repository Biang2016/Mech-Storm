using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class BonusButton : PoolObject
{
    public override void PoolRecycle()
    {
        foreach (BonusItem_Base bonusItem in M_BonusItems)
        {
            bonusItem.PoolRecycle();
        }

        SetSelected(false);
        base.PoolRecycle();
    }

    public BonusGroup BonusGroup;

    [SerializeField] private Image BG_Image;
    [SerializeField] private Transform Container;
    [SerializeField] private Button Button;
    [SerializeField] private Image BorderSelected;

    private List<BonusItem_Base> M_BonusItems = new List<BonusItem_Base>();

    public void Initialize(BonusGroup bonusGroup)
    {
        foreach (BonusItem_Base bonusItem in M_BonusItems)
        {
            bonusItem.PoolRecycle();
        }

        M_BonusItems.Clear();

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
    }

    public void OnButtonClick()
    {
        WinLostPanelManager.Instance.SetBonusButtonSelected(this);
        AudioManager.Instance.SoundPlay("sfx/OnStoryButtonClick");
    }

    public void OnConfirmButtonClickDelegate()
    {
        BonusGroupRequest request = new BonusGroupRequest(Client.Instance.Proxy.ClientId, BonusGroup);
        Client.Instance.Proxy.SendMessage(request);
    }

    public void SetSelected(bool isSelected)
    {
        BorderSelected.enabled = isSelected;
    }
}