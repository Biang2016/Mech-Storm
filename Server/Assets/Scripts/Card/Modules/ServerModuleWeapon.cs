using System.Collections.Generic;
using UnityEngine;

internal class ServerModuleWeapon : ServerModuleBase
{
    #region 各模块、自身数值和初始化

    internal ServerModuleRetinue M_ModuleRetinue;

    public override void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        M_WeaponName = CardInfo_Base.textToVertical(((CardInfo_Weapon) cardInfo).BaseInfo.CardName);
        M_WeaponType = ((CardInfo_Weapon) cardInfo).WeaponInfo.WeaponType;
        M_WeaponAttack = ((CardInfo_Weapon) cardInfo).WeaponInfo.Attack;
        M_WeaponEnergyMax = ((CardInfo_Weapon) cardInfo).WeaponInfo.EnergyMax;
        M_WeaponEnergy = ((CardInfo_Weapon) cardInfo).WeaponInfo.Energy;
        base.Initiate(cardInfo, serverPlayer);
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Weapon(CardInfo.CardID,CardInfo.BaseInfo,CardInfo.UpgradeInfo,CardInfo.WeaponInfo,CardInfo.SideEffects_OnDie);
    }


    #region 属性

    private int m_WeaponPlaceIndex;

    public int M_WeaponPlaceIndex
    {
        get { return m_WeaponPlaceIndex; }
        set { m_WeaponPlaceIndex = value; }
    }

    private string m_WeaponName;

    public string M_WeaponName
    {
        get { return m_WeaponName; }

        set { m_WeaponName = value; }
    }

    private WeaponTypes m_WeaponType;

    public WeaponTypes M_WeaponType
    {
        get { return m_WeaponType; }

        set { m_WeaponType = value; }
    }

    private int m_WeaponAttack;

    public int M_WeaponAttack
    {
        get { return m_WeaponAttack; }

        set
        {
            int before = m_WeaponAttack;
            m_WeaponAttack = value;
            if (isInitialized && before != m_WeaponAttack)
            {
                WeaponAttributesChangeRequest request = new WeaponAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_WeaponPlaceIndex, WeaponAttributesChangeRequest.WeaponAttributesChangeFlag.Attack, m_WeaponAttack - before, 0);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_WeaponEnergy;

    public int M_WeaponEnergy
    {
        get { return m_WeaponEnergy; }

        set
        {
            int before = m_WeaponEnergy;
            m_WeaponEnergy = Mathf.Min(value, M_WeaponEnergyMax);
            if (isInitialized && before != m_WeaponEnergy)
            {
                WeaponAttributesChangeRequest request = new WeaponAttributesChangeRequest(ServerPlayer.ClientId, M_RetinuePlaceIndex, M_WeaponPlaceIndex, WeaponAttributesChangeRequest.WeaponAttributesChangeFlag.Energy, 0, m_WeaponEnergy - before);
                ServerPlayer.MyEnemyPlayer.MyClientProxy.MyServerGameManager.Broadcast_AddRequestToOperationResponse(request);
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

    #endregion

    #region 模块交互

    #region  武器更换

    public void ChangeWeapon(ServerModuleWeapon newWeapon, ref ServerModuleWeapon resultWeapon)
    {
        if (AllCards.IsASeries(CardInfo, newWeapon.CardInfo))
        {
            if (CardInfo.UpgradeInfo.CardLevel == newWeapon.CardInfo.UpgradeInfo.CardLevel)
            {
                CardInfo_Weapon m_currentInfo = (CardInfo_Weapon) GetCurrentCardInfo();
                CardInfo_Weapon upgradeWeaponCardInfo = (CardInfo_Weapon) AllCards.GetCard(CardInfo.UpgradeInfo.UpgradeCardID);
                Initiate(upgradeWeaponCardInfo, ServerPlayer);
                M_WeaponAttack = m_currentInfo.WeaponInfo.Attack + newWeapon.CardInfo.WeaponInfo.Attack;
                M_WeaponEnergy = m_currentInfo.WeaponInfo.Energy + newWeapon.CardInfo.WeaponInfo.Energy;
                //newWeapon.PoolRecycle();
                resultWeapon = this;
            }
            else if (CardInfo.UpgradeInfo.CardLevel > newWeapon.CardInfo.UpgradeInfo.CardLevel)
            {
                M_WeaponAttack = M_WeaponAttack + newWeapon.CardInfo.WeaponInfo.Attack;
                M_WeaponEnergy = M_WeaponEnergy + newWeapon.CardInfo.WeaponInfo.Energy;
                //newWeapon.PoolRecycle();
                resultWeapon = this;
            }
            else
            {
                resultWeapon = newWeapon;
                newWeapon.M_WeaponAttack = M_WeaponAttack + newWeapon.CardInfo.WeaponInfo.Attack;
                newWeapon.M_WeaponEnergy = M_WeaponEnergy + newWeapon.CardInfo.WeaponInfo.Energy;
                //PoolRecycle();
            }
        }
        else
        {
            resultWeapon = newWeapon;
            //if (RoundManager.RM.CurrentClientPlayer == ServerPlayer)
            //{
            //    M_ModuleRetinue.CanAttack = true;
            //    newWeapon.CanAttack = true;
            //}

            //PoolRecycle();
        }
    }

    #endregion

    #region 攻击相关

    internal bool CanAttack;

    public List<int> WeaponAttack()
    {
        CanAttack = false;
        List<int> aSeriesOfAttacks = new List<int>();
        switch (M_WeaponType)
        {
            case WeaponTypes.Sword:
                aSeriesOfAttacks.Add((M_WeaponAttack + M_ModuleRetinue.M_RetinueAttack) * M_WeaponEnergy);
                if (M_WeaponEnergy < M_WeaponEnergyMax) M_WeaponEnergy++;
                return aSeriesOfAttacks;
            case WeaponTypes.Gun:
                int tmp = M_WeaponEnergy;
                for (int i = 0; i < tmp; i++)
                {
                    aSeriesOfAttacks.Add(M_WeaponAttack + M_ModuleRetinue.M_RetinueAttack);
                    M_WeaponEnergy--;
                }

                if (M_WeaponEnergy == 0)
                {
                    M_ModuleRetinue.CanAttack = false;
                    CanAttack = false;
                }

                return aSeriesOfAttacks;
            default:
                return aSeriesOfAttacks;
        }
    }

    #endregion

    #endregion
}