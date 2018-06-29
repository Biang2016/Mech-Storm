using System.Collections.Generic;
using UnityEngine;

public class ModuleWeapon : ModuleBase
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_ModuleWeaponPool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 各模块、自身数值和初始化

    internal ModuleRetinue M_ModuleRetinue;
    public TextMesh WeaponName;
    public GameObject M_Bloom;
    public GameObject M_GunIcon;
    public GameObject M_SwordIcon;


    public GameObject Block_WeaponAttack;
    protected GameObject GoNumberSet_WeaponAttack;
    protected CardNumberSet CardNumberSet_WeaponAttack;

    public GameObject Block_WeaponEnergy;
    protected GameObject GoNumberSet_WeaponEnergy;
    protected CardNumberSet CardNumberSet_WeaponEnergy;

    public override void Initiate(CardInfo_Base cardInfo, Player player)
    {
        base.Initiate(cardInfo, player);
        M_WeaponName = CardInfo_Weapon.textToVertical(((CardInfo_Weapon) cardInfo).CardName);
        M_WeaponType = ((CardInfo_Weapon) cardInfo).M_WeaponType;
        M_WeaponAttack = ((CardInfo_Weapon) cardInfo).Attack;
        M_WeaponEnergy = ((CardInfo_Weapon) cardInfo).Energy;
        M_WeaponEnergyMax = ((CardInfo_Weapon) cardInfo).EnergyMax;
        if (M_Bloom) M_Bloom.SetActive(false);
        if (M_WeaponType == WeaponType.Gun)
        {
            if (M_GunIcon) M_GunIcon.SetActive(true);
            if (M_SwordIcon) M_SwordIcon.SetActive(false);
        }
        else if (M_WeaponType == WeaponType.Sword)
        {
            if (M_GunIcon) M_GunIcon.SetActive(false);
            if (M_SwordIcon) M_SwordIcon.SetActive(true);
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Weapon(CardInfo.CardID, CardInfo.CardName, CardInfo.CardDesc, CardInfo.Cost, CardInfo.HasTarget, CardInfo.CardType, CardInfo.CardColor, CardInfo.UpgradeID, CardInfo.CardLevel, M_WeaponEnergy, M_WeaponEnergyMax, M_WeaponAttack, M_WeaponType);
    }

    private string m_WeaponName;

    public string M_WeaponName
    {
        get { return m_WeaponName; }

        set
        {
            m_WeaponName = value;
            WeaponName.text = m_WeaponName;
        }
    }

    private WeaponType m_WeaponType;

    public WeaponType M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }

    private NumberSize my_NumberSize_Attack = NumberSize.Big;
    private NumberSize my_NumberSize_Energy = NumberSize.Medium;
    private CardNumberSet.TextAlign my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
    private CardNumberSet.TextAlign my_TextAlign_Energy = CardNumberSet.TextAlign.Center;

    public void SetPreview()
    {
        my_NumberSize_Attack = NumberSize.Small;
        my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
        my_NumberSize_Energy = NumberSize.Small;
        my_TextAlign_Energy = CardNumberSet.TextAlign.Center;
        M_WeaponAttack = M_WeaponAttack;
        M_WeaponEnergy = M_WeaponEnergy;
    }

    public void SetNoPreview()
    {
        my_NumberSize_Attack = NumberSize.Big;
        my_TextAlign_Attack = CardNumberSet.TextAlign.Center;
        my_NumberSize_Energy = NumberSize.Medium;
        my_TextAlign_Energy = CardNumberSet.TextAlign.Center;
        M_WeaponEnergy = M_WeaponEnergy;
        M_WeaponEnergy = M_WeaponEnergy;
    }

    private int m_WeaponAttack;

    public int M_WeaponAttack
    {
        get { return m_WeaponAttack; }

        set
        {
            m_WeaponAttack = value;
            initiateNumbers(ref GoNumberSet_WeaponAttack, ref CardNumberSet_WeaponAttack, my_NumberSize_Attack, my_TextAlign_Attack, Block_WeaponAttack);
            CardNumberSet_WeaponAttack.Number = m_WeaponAttack + M_ModuleRetinue.M_RetinueAttack;
            if (m_WeaponAttack + M_ModuleRetinue.M_RetinueAttack == 0)
            {
                GetComponent<DragComponent>().enabled = false;
            }
            else
            {
                GetComponent<DragComponent>().enabled = true;
            }
        }
    }

    private int m_WeaponEnergy;

    public int M_WeaponEnergy
    {
        get { return m_WeaponEnergy; }

        set
        {
            m_WeaponEnergy = value;
            if (M_WeaponType == WeaponType.Sword)
            {
                initiateNumbers(ref GoNumberSet_WeaponEnergy, ref CardNumberSet_WeaponEnergy, my_NumberSize_Energy, my_TextAlign_Energy, Block_WeaponEnergy, 'x');
            }
            else if (M_WeaponType == WeaponType.Gun)
            {
                initiateNumbers(ref GoNumberSet_WeaponEnergy, ref CardNumberSet_WeaponEnergy, my_NumberSize_Energy, my_TextAlign_Energy, Block_WeaponEnergy, 'x');
            }

            CardNumberSet_WeaponEnergy.Number = m_WeaponEnergy;
            if (m_WeaponEnergy == 0)
            {
                GetComponent<DragComponent>().enabled = false;
            }
            else
            {
                GetComponent<DragComponent>().enabled = true;
            }
        }
    }

    private int m_WeaponEnergyMax;

    public int M_WeaponEnergyMax
    {
        get { return m_WeaponEnergyMax; }

        set { m_WeaponEnergyMax = value; }
    }

    #endregion

    #region 模块交互

    #region 攻击相关

    internal bool CanAttack;

    public List<int> WeaponAttack()
    {
        CanAttack = false;
        var aSeriesOfAttacks = new List<int>();
        switch (M_WeaponType)
        {
            case WeaponType.Sword:
                aSeriesOfAttacks.Add((M_WeaponAttack + M_ModuleRetinue.M_RetinueAttack) * M_WeaponEnergy);
                if (M_WeaponEnergy < M_WeaponEnergyMax) M_WeaponEnergy++;
                return aSeriesOfAttacks;
            case WeaponType.Gun:
                int tmp = M_WeaponEnergy;
                for (int i = 0; i < tmp; i++)
                {
                    aSeriesOfAttacks.Add(M_WeaponAttack + M_ModuleRetinue.M_RetinueAttack);
                    M_WeaponEnergy--;
                }

                return aSeriesOfAttacks;
            default:
                return aSeriesOfAttacks;
        }
    }


    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (moduleRetinue && moduleRetinue.Player != Player)
        {
            var aSeriesOfAttacks = WeaponAttack();
            M_ModuleRetinue.CanAttack = false;
            foreach (var attackNumber in aSeriesOfAttacks) moduleRetinue.BeAttacked(attackNumber);
        }
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref bool hasTarget)
    {
        canDrag = CanAttack;
        hasTarget = CardInfo.HasTarget;
    }


    public override float DragComponnet_DragDistance()
    {
        return 0.2f;
    }

    #endregion

    #region 交互UX

    public override void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMouseEnterImmediately(mousePosition);
        if (M_Bloom) M_Bloom.SetActive(true);
    }

    public override void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        base.MouseHoverComponent_OnMouseLeaveImmediately();
        if (M_Bloom) M_Bloom.SetActive(false);
    }

    #endregion

    #endregion
}

public enum WeaponType
{
    Sword = 0,
    Gun = 1,
}