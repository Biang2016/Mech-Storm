using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCardDetailInBattleManager : MonoSingleton<ShowCardDetailInBattleManager>
{
    private ShowCardDetailInBattleManager()
    {
    }

    private CardBase detailCard;
    private CardBase detailCard_Weapon;
    private CardBase detailCard_Shield;
    private CardBase detailCard_Pack;
    private CardBase detailCard_MA;

    public void ShowCardDetail(ModuleBase moduleBase) //鼠标悬停放大查看原卡牌信息
    {
        CardInfo_Base CardInfo = moduleBase.CardInfo;
        switch (CardInfo.BaseInfo.CardType)
        {
            case CardTypes.Retinue:
                detailCard = (CardRetinue) CardBase.InstantiateCardByCardInfo(CardInfo, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                detailCard.transform.localScale = Vector3.one * GameManager.Instance.DetailRetinueCardSize;
                detailCard.transform.position = new Vector3(0, 8f, 0);
                detailCard.transform.Translate(Vector3.left * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.GetComponent<DragComponent>().enabled = false;
                detailCard.BeBrightColor();
                detailCard.SetOrderInLayer(200);

                CardRetinue cardRetinue = (CardRetinue) detailCard;
                //cardRetinue.ShowAllSlotHover();

                if (((ModuleRetinue) moduleBase).M_Weapon)
                {
                    if (!cardRetinue.Weapon)
                    {
                        cardRetinue.Weapon = GameObjectPoolManager.Instance.Pool_ModuleWeaponDetailPool.AllocateGameObject<ModuleWeapon>(cardRetinue.transform);
                    }

                    CardInfo_Base cw = ((ModuleRetinue) moduleBase).M_Weapon.CardInfo;
                    cardRetinue.Weapon.M_ModuleRetinue = (ModuleRetinue) moduleBase;
                    cardRetinue.Weapon.Initiate(((ModuleRetinue) moduleBase).M_Weapon.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardRetinue.Weapon.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Weapon.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Weapon.SetPreview();

                    detailCard_Weapon = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                    detailCard_Weapon.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Weapon.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Weapon.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Weapon.transform.Translate(Vector3.back * 3f);
                    detailCard_Weapon.transform.Translate(Vector3.up * 7f);
                    detailCard_Weapon.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Weapon.BeBrightColor();
                    detailCard_Weapon.CardBloom.SetActive(true);
                    detailCard_Weapon.SetOrderInLayer(200);
                }

                if (((ModuleRetinue) moduleBase).M_Shield)
                {
                    if (!cardRetinue.Shield)
                    {
                        cardRetinue.Shield = GameObjectPoolManager.Instance.Pool_ModuleShieldDetailPool.AllocateGameObject<ModuleShield>(cardRetinue.transform);
                    }

                    CardInfo_Base cw = ((ModuleRetinue) moduleBase).M_Shield.CardInfo;
                    cardRetinue.Shield.M_ModuleRetinue = (ModuleRetinue) moduleBase;
                    cardRetinue.Shield.Initiate(((ModuleRetinue) moduleBase).M_Shield.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardRetinue.Shield.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Shield.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Shield.SetPreview();

                    detailCard_Shield = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                    detailCard_Shield.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Shield.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Shield.transform.Translate(Vector3.right * 0.5f);
                    detailCard_Shield.transform.Translate(Vector3.forward * 3f);
                    detailCard_Shield.transform.Translate(Vector3.up * 7f);
                    detailCard_Shield.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Shield.BeBrightColor();
                    detailCard_Shield.CardBloom.SetActive(true);
                    detailCard_Shield.SetOrderInLayer(200);
                }

                if (((ModuleRetinue) moduleBase).M_Pack)
                {
                    if (!cardRetinue.Pack)
                    {
                        cardRetinue.Pack = GameObjectPoolManager.Instance.Pool_ModulePackDetailPool.AllocateGameObject<ModulePack>(cardRetinue.transform);
                    }

                    CardInfo_Base cw = ((ModuleRetinue) moduleBase).M_Pack.CardInfo;
                    cardRetinue.Pack.M_ModuleRetinue = (ModuleRetinue) moduleBase;
                    cardRetinue.Pack.Initiate(((ModuleRetinue) moduleBase).M_Pack.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardRetinue.Pack.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.Pack.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.Pack.SetPreview();

                    detailCard_Pack = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                    detailCard_Pack.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_Pack.transform.position = new Vector3(0, 2f, 0);
                    detailCard_Pack.transform.Translate(Vector3.left * 10.5f);
                    detailCard_Pack.transform.Translate(Vector3.back * 3f);
                    detailCard_Pack.transform.Translate(Vector3.up * 7f);
                    detailCard_Pack.GetComponent<BoxCollider>().enabled = false;
                    detailCard_Pack.BeBrightColor();
                    detailCard_Pack.CardBloom.SetActive(true);
                    detailCard_Pack.SetOrderInLayer(200);
                }

                if (((ModuleRetinue) moduleBase).M_MA)
                {
                    if (!cardRetinue.MA)
                    {
                        cardRetinue.MA = GameObjectPoolManager.Instance.Pool_ModuleMADetailPool.AllocateGameObject<ModuleMA>(cardRetinue.transform);
                    }

                    CardInfo_Base cw = ((ModuleRetinue) moduleBase).M_MA.CardInfo;
                    cardRetinue.MA.M_ModuleRetinue = (ModuleRetinue) moduleBase;
                    cardRetinue.MA.Initiate(((ModuleRetinue) moduleBase).M_MA.GetCurrentCardInfo(), moduleBase.ClientPlayer);
                    cardRetinue.MA.GetComponent<DragComponent>().enabled = false;
                    cardRetinue.MA.GetComponent<MouseHoverComponent>().enabled = false;
                    cardRetinue.MA.SetPreview();

                    detailCard_MA = (CardEquip) CardBase.InstantiateCardByCardInfo(cw, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                    detailCard_MA.transform.localScale = Vector3.one * GameManager.Instance.DetailEquipmentCardSize;
                    detailCard_MA.transform.position = new Vector3(0, 2f, 0);
                    detailCard_MA.transform.Translate(Vector3.left * 10.5f);
                    detailCard_MA.transform.Translate(Vector3.forward * 3f);
                    detailCard_MA.transform.Translate(Vector3.up * 7f);
                    detailCard_MA.GetComponent<BoxCollider>().enabled = false;
                    detailCard_MA.BeBrightColor();
                    detailCard_MA.CardBloom.SetActive(true);
                    detailCard_MA.SetOrderInLayer(200);
                }

                break;
            case CardTypes.Equip:
                detailCard = (CardEquip) CardBase.InstantiateCardByCardInfo(CardInfo, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
                detailCard.transform.localScale = Vector3.one * GameManager.Instance.DetailSingleCardSize;
                detailCard.transform.position = new Vector3(0, 2f, 0);
                detailCard.transform.Translate(Vector3.left * 3.5f);
                detailCard.transform.Translate(Vector3.up * 5f);
                detailCard.GetComponent<BoxCollider>().enabled = false;
                detailCard.BeBrightColor();
                detailCard.SetOrderInLayer(200);
                break;
            default:
                break;
        }

        detailCard.CardBloom.SetActive(true);
        List<CardInfo_Base> cardInfos = new List<CardInfo_Base>();
        if (detailCard != null) cardInfos.Add(detailCard.CardInfo);
        if (detailCard_Weapon != null) cardInfos.Add(detailCard_Weapon.CardInfo);
        if (detailCard_Shield != null) cardInfos.Add(detailCard_Shield.CardInfo);
        if (detailCard_Pack != null) cardInfos.Add(detailCard_Pack.CardInfo);
        if (detailCard_MA != null) cardInfos.Add(detailCard_MA.CardInfo);
        AffixManager.Instance.ShowAffixTips(cardInfos, moduleBase is ModuleRetinue ? new List<ModuleRetinue> {(ModuleRetinue) moduleBase} : null);
    }

    public enum ShowPlaces
    {
        RightCenter,
        LeftLower,
        RightUpper,
    }

    public void ShowCardDetail(CardInfo_Base CardInfo, ShowPlaces showPlace = ShowPlaces.RightCenter) //鼠标悬停放大查看原卡牌信息
    {
        detailCard = CardBase.InstantiateCardByCardInfo(CardInfo, GameBoardManager.Instance.CardDetailPreview.transform, RoundManager.Instance.SelfClientPlayer, false);
        detailCard.transform.localScale = Vector3.one * GameManager.Instance.DetailSingleCardSize;
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
        detailCard.SetOrderInLayer(200);
        detailCard.CardBloom.SetActive(true);
        List<CardInfo_Base> cardInfos = new List<CardInfo_Base>();
        if (detailCard != null) cardInfos.Add(detailCard.CardInfo);
        AffixManager.Instance.ShowAffixTips(cardInfos, null);
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

        AffixManager.Instance.HideAffixPanel();
    }
}