using System.Collections;
using UnityEngine;

public class ModuleMA : ModuleEquip
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModuleMAPool;
    }


    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_MAName = GameManager.Instance.isEnglish ? cardInfo.BaseInfo.CardName_en : cardInfo.BaseInfo.CardName;
    }

    public override void SetPreview()
    {
        base.SetPreview();
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
    }

    public CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        return currentCI;
    }

    private string m_MAName;

    public string M_MAName
    {
        get { return m_MAName; }

        set
        {
            m_MAName = value;
            Name.text = GameManager.Instance.isEnglish ? "" : value;
            Name_en.text = GameManager.Instance.isEnglish ? value : "";
        }
    }


    public void OnMAEquiped()
    {
        EquipAnim.SetTrigger("MAEquiped");
    }
}