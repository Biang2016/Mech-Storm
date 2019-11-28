using System.Collections.Generic;
using UnityEngine;

public class ShowCardDetailInBattleManager : MonoBehaviour
{
    private CardBase detailCard;
    private CardBase detailCard_Weapon;
    private CardBase detailCard_Shield;
    private CardBase detailCard_Pack;
    private CardBase detailCard_MA;

    private float DETAIL_SINGLE_CARD_SIZE = 0.3f;
    private float DETAIL_EQUIPMENT_CARD_SIZE = 0.25f;
    private float DETAIL_MECH_CARD_SIZE = 0.4f;
    private readonly Vector3 DETAIL_ROTATION = new Vector3(-90, -180, 0);

    [SerializeField] private Transform DetailMechCardPivot;
    [SerializeField] private Transform DetailMechWeaponCardPivot;
    [SerializeField] private Transform DetailMechShieldCardPivot;
    [SerializeField] private Transform DetailMechPackCardPivot;
    [SerializeField] private Transform DetailMechMACardPivot;
    [SerializeField] private Transform DetailOtherCardPivot;

    public void ShowCardDetail(ModuleBase moduleBase) //鼠标悬停放大查看原卡牌信息
    {
        CardInfo_Base CardInfo = moduleBase.CardInfo;
        switch (CardInfo.BaseInfo.CardType)
        {
            case CardTypes.Mech:
                detailCard = (CardMech) CardBase.InstantiateCardByCardInfo(CardInfo, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                detailCard.transform.localScale = Vector3.one * DETAIL_MECH_CARD_SIZE;
                detailCard.transform.position = DetailMechCardPivot.position;
                detailCard.BoxCollider.enabled = false;
                detailCard.DragComponent.enabled = false;
                detailCard.BeBrightColor();
                detailCard.CardOrder = 200;

                CardMech cardMech = (CardMech) detailCard;
                //cardMech.ShowAllSlotHover();

                if (((ModuleMech) moduleBase).MechEquipSystemComponent.M_Weapon)
                {
                    if (!cardMech.Weapon)
                    {
                        cardMech.Weapon = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleWeaponDetail].AllocateGameObject<ModuleWeapon>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).MechEquipSystemComponent.M_Weapon.CardInfo;

                    cardMech.Weapon.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Weapon.Initiate(((ModuleMech) moduleBase).MechEquipSystemComponent.M_Weapon.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Weapon.DragComponent.enabled = false;
                    cardMech.Weapon.MouseHoverComponent.enabled = false;
                    cardMech.Weapon.SetPreview();

                    detailCard_Weapon = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Weapon.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
                    detailCard_Weapon.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Weapon.transform.position = DetailMechWeaponCardPivot.position;
                    detailCard_Weapon.BoxCollider.enabled = false;
                    detailCard_Weapon.BeBrightColor();
                    detailCard_Weapon.ShowCardBloom(true);
                    detailCard_Weapon.CardOrder = 200;
                }

                if (((ModuleMech) moduleBase).MechEquipSystemComponent.M_Shield)
                {
                    if (!cardMech.Shield)
                    {
                        cardMech.Shield = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleShieldDetail].AllocateGameObject<ModuleShield>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).MechEquipSystemComponent.M_Shield.CardInfo;
                    cardMech.Shield.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Shield.Initiate(((ModuleMech) moduleBase).MechEquipSystemComponent.M_Shield.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Shield.DragComponent.enabled = false;
                    cardMech.Shield.MouseHoverComponent.enabled = false;
                    cardMech.Shield.SetPreview();

                    detailCard_Shield = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Shield.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
                    detailCard_Shield.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Shield.transform.position = DetailMechShieldCardPivot.position;
                    detailCard_Shield.BoxCollider.enabled = false;
                    detailCard_Shield.BeBrightColor();
                    detailCard_Shield.ShowCardBloom(true);
                    detailCard_Shield.CardOrder = 200;
                }

                if (((ModuleMech) moduleBase).MechEquipSystemComponent.M_Pack)
                {
                    if (!cardMech.Pack)
                    {
                        cardMech.Pack = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModulePackDetail].AllocateGameObject<ModulePack>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).MechEquipSystemComponent.M_Pack.CardInfo;
                    cardMech.Pack.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Pack.Initiate(((ModuleMech) moduleBase).MechEquipSystemComponent.M_Pack.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Pack.DragComponent.enabled = false;
                    cardMech.Pack.MouseHoverComponent.enabled = false;
                    cardMech.Pack.SetPreview();

                    detailCard_Pack = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Pack.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
                    detailCard_Pack.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Pack.transform.position = DetailMechPackCardPivot.position;
                    detailCard_Pack.BoxCollider.enabled = false;
                    detailCard_Pack.BeBrightColor();
                    detailCard_Pack.ShowCardBloom(true);
                    detailCard_Pack.CardOrder = 200;
                }

                if (((ModuleMech) moduleBase).MechEquipSystemComponent.M_MA)
                {
                    if (!cardMech.MA)
                    {
                        cardMech.MA = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleMADetail].AllocateGameObject<ModuleMA>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).MechEquipSystemComponent.M_MA.CardInfo;
                    cardMech.MA.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.MA.Initiate(((ModuleMech) moduleBase).MechEquipSystemComponent.M_MA.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.MA.DragComponent.enabled = false;
                    cardMech.MA.MouseHoverComponent.enabled = false;
                    cardMech.MA.SetPreview();

                    detailCard_MA = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_MA.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
                    detailCard_MA.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_MA.transform.position = DetailMechMACardPivot.position;
                    detailCard_MA.BoxCollider.enabled = false;
                    detailCard_MA.BeBrightColor();
                    detailCard_MA.ShowCardBloom(true);
                    detailCard_MA.CardOrder = 200;
                }

                cardMech.SetEquipInPlace();

                break;
            case CardTypes.Equip:
                detailCard = (CardEquip) CardBase.InstantiateCardByCardInfo(CardInfo, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                detailCard.transform.localScale = Vector3.one * DETAIL_SINGLE_CARD_SIZE;
                detailCard.transform.position = DetailOtherCardPivot.position;
                detailCard.BoxCollider.enabled = false;
                detailCard.BeBrightColor();
                detailCard.CardOrder = 200;
                break;
        }

        detailCard.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
        detailCard.ShowCardBloom(true);
        if (detailCard is CardMech mechCard) mechCard.ShowEquipCardBloom();

        List<CardInfo_Base> cardInfos = new List<CardInfo_Base>();
        if (detailCard != null) cardInfos.Add(detailCard.CardInfo);
        if (detailCard_Weapon != null) cardInfos.Add(detailCard_Weapon.CardInfo);
        if (detailCard_Shield != null) cardInfos.Add(detailCard_Shield.CardInfo);
        if (detailCard_Pack != null) cardInfos.Add(detailCard_Pack.CardInfo);
        if (detailCard_MA != null) cardInfos.Add(detailCard_MA.CardInfo);
        UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(cardInfos, moduleBase is ModuleMech ? new List<ModuleMech> {(ModuleMech) moduleBase} : null);
    }

    public enum ShowPlaces
    {
        RightCenter,
        LeftLower,
        RightUpper,
    }

    public void ShowCardDetail(CardInfo_Base CardInfo, ShowPlaces showPlace = ShowPlaces.RightCenter) //鼠标悬停放大查看原卡牌信息
    {
        detailCard = CardBase.InstantiateCardByCardInfo(CardInfo, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
        detailCard.transform.localScale = Vector3.one * DETAIL_SINGLE_CARD_SIZE;
        Vector3 targetPos;
        switch (showPlace)
        {
            case ShowPlaces.RightCenter:
            {
                targetPos = new Vector3(-3.5f, 7f, 0);
                break;
            }
            case ShowPlaces.LeftLower:
            {
                targetPos = new Vector3(-5.5f, 7f, -3.3f);
                break;
            }
            case ShowPlaces.RightUpper:
            {
                targetPos = new Vector3(7.5f, 7f, 3.3f);
                break;
            }
            default:
            {
                targetPos = new Vector3(-3.5f, 7f, 0);
                break;
            }
        }

        detailCard.transform.rotation = Quaternion.Euler(DETAIL_ROTATION.x, DETAIL_ROTATION.y, DETAIL_ROTATION.z);
        detailCard.transform.position = targetPos;
        detailCard.BoxCollider.enabled = false;
        detailCard.BeBrightColor();
        detailCard.CardOrder = 200;
        detailCard.ShowCardBloom(true);
        List<CardInfo_Base> cardInfos = new List<CardInfo_Base>();
        if (detailCard != null) cardInfos.Add(detailCard.CardInfo);
        UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(cardInfos, null);
    }

    public void HideCardDetail()
    {
        if (detailCard)
        {
            detailCard.PoolRecycle();
            detailCard = null;
        }

        if (detailCard_Weapon)
        {
            detailCard_Weapon.PoolRecycle();
            detailCard_Weapon = null;
        }

        if (detailCard_Shield)
        {
            detailCard_Shield.PoolRecycle();
            detailCard_Shield = null;
        }

        if (detailCard_Pack)
        {
            detailCard_Pack.PoolRecycle();
            detailCard_Pack = null;
        }

        if (detailCard_MA)
        {
            detailCard_MA.PoolRecycle();
            detailCard_MA = null;
        }

        UIManager.Instance.CloseUIForm<AffixPanel>();
    }
}