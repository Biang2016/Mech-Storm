using System.Collections.Generic;
using UnityEngine;

public class ShowCardDetailInBattleManager : MonoBehaviour
{
    private CardBase detailCard;
    private CardBase detailCard_Weapon;
    private CardBase detailCard_Shield;
    private CardBase detailCard_Pack;
    private CardBase detailCard_MA;
    
    private float DETAIL_SINGLE_CARD_SIZE = 3.0f;
    private float DETAIL_EQUIPMENT_CARD_SIZE = 2.5f;
    private float DETAIL_MECH_CARD_SIZE = 4.0f;

    public void ShowCardDetail(ModuleBase moduleBase) //鼠标悬停放大查看原卡牌信息
    {
        CardInfo_Base CardInfo = moduleBase.CardInfo;
        switch (CardInfo.BaseInfo.CardType)
        {
            case CardTypes.Mech:
                detailCard = (CardMech) CardBase.InstantiateCardByCardInfo(CardInfo, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                detailCard.transform.localScale = Vector3.one * DETAIL_MECH_CARD_SIZE;
                detailCard.transform.position = new Vector3(0, 8f, 0);
                detailCard.transform.Translate(Vector3.left * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.GetComponent<DragComponent>().enabled = false;
                detailCard.BeBrightColor();
                detailCard.CardOrder = 100;

                CardMech cardMech = (CardMech) detailCard;
                //cardMech.ShowAllSlotHover();

                if (((ModuleMech) moduleBase).M_Weapon)
                {
                    if (!cardMech.Weapon)
                    {
                        cardMech.Weapon = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleWeaponDetail].AllocateGameObject<ModuleWeapon>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).M_Weapon.CardInfo;
                    cardMech.Weapon.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Weapon.Initiate(((ModuleMech) moduleBase).M_Weapon.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Weapon.GetComponent<DragComponent>().enabled = false;
                    cardMech.Weapon.GetComponent<MouseHoverComponent>().enabled = false;
                    cardMech.Weapon.SetPreview();

                    detailCard_Weapon = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Weapon.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Weapon.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Weapon.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Weapon.transform.Translate(Vector3.back * 3f);
                    detailCard_Weapon.transform.Translate(Vector3.up * 7f);
                    detailCard_Weapon.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Weapon.BeBrightColor();
                    detailCard_Weapon.ShowCardBloom(true);
                    detailCard_Weapon.CardOrder = 100;
                }

                if (((ModuleMech) moduleBase).M_Shield)
                {
                    if (!cardMech.Shield)
                    {
                        cardMech.Shield = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleShieldDetail].AllocateGameObject<ModuleShield>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).M_Shield.CardInfo;
                    cardMech.Shield.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Shield.Initiate(((ModuleMech) moduleBase).M_Shield.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Shield.GetComponent<DragComponent>().enabled = false;
                    cardMech.Shield.GetComponent<MouseHoverComponent>().enabled = false;
                    cardMech.Shield.SetPreview();

                    detailCard_Shield = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Shield.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Shield.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Shield.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Shield.transform.Translate(Vector3.forward * 3f);
                    detailCard_Shield.transform.Translate(Vector3.up * 7f);
                    detailCard_Shield.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Shield.BeBrightColor();
                    detailCard_Shield.ShowCardBloom(true);
                    detailCard_Shield.CardOrder = 100;
                }

                if (((ModuleMech) moduleBase).M_Pack)
                {
                    if (!cardMech.Pack)
                    {
                        cardMech.Pack = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModulePackDetail].AllocateGameObject<ModulePack>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).M_Pack.CardInfo;
                    cardMech.Pack.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.Pack.Initiate(((ModuleMech) moduleBase).M_Pack.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.Pack.GetComponent<DragComponent>().enabled = false;
                    cardMech.Pack.GetComponent<MouseHoverComponent>().enabled = false;
                    cardMech.Pack.SetPreview();

                    detailCard_Pack = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_Pack.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_Pack.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Pack.transform.Translate(Vector3.left * 10.5f);
                    detailCard_Pack.transform.Translate(Vector3.back * 3f);
                    detailCard_Pack.transform.Translate(Vector3.up * 7f);
                    detailCard_Pack.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Pack.BeBrightColor();
                    detailCard_Pack.ShowCardBloom(true);
                    detailCard_Pack.CardOrder = 100;
                }

                if (((ModuleMech) moduleBase).M_MA)
                {
                    if (!cardMech.MA)
                    {
                        cardMech.MA = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleMADetail].AllocateGameObject<ModuleMA>(cardMech.transform);
                    }

                    CardInfo_Base cw = ((ModuleMech) moduleBase).M_MA.CardInfo;
                    cardMech.MA.M_ModuleMech = (ModuleMech) moduleBase;
                    cardMech.MA.Initiate(((ModuleMech) moduleBase).M_MA.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardMech.MA.GetComponent<DragComponent>().enabled = false;
                    cardMech.MA.GetComponent<MouseHoverComponent>().enabled = false;
                    cardMech.MA.SetPreview();

                    detailCard_MA = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                    detailCard_MA.transform.localScale = Vector3.one * DETAIL_EQUIPMENT_CARD_SIZE;
                    detailCard_MA.transform.position = new Vector3(0, 2f, 0);
                    detailCard_MA.transform.Translate(Vector3.left * 10.5f);
                    detailCard_MA.transform.Translate(Vector3.forward * 3f);
                    detailCard_MA.transform.Translate(Vector3.up * 7f);
                    detailCard_MA.GetComponent<BoxCollider>().enabled = false;
                    detailCard_MA.BeBrightColor();
                    detailCard_MA.ShowCardBloom(true);
                    detailCard_MA.CardOrder = 100;
                }

                break;
            case CardTypes.Equip:
                detailCard = (CardEquip) CardBase.InstantiateCardByCardInfo(CardInfo, BattleManager.Instance.ShowCardDetailInBattleManager.transform, CardBase.CardShowMode.CardPreviewBattle, RoundManager.Instance.SelfClientPlayer);
                detailCard.transform.localScale = Vector3.one * DETAIL_SINGLE_CARD_SIZE;
                detailCard.transform.position = new Vector3(0, 2f, 0);
                detailCard.transform.Translate(Vector3.left * 3.5f);
                detailCard.transform.Translate(Vector3.up * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.BeBrightColor();
                detailCard.CardOrder = 100;
                break;
            default:
                break;
        }

        detailCard.ShowCardBloom(true);
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

        detailCard.transform.position = targetPos;
        detailCard.GetComponent<BoxCollider>().enabled = false;
        detailCard.BeBrightColor();
        detailCard.CardOrder = 100;
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