using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ClientPlayer : Player
{
    public BattlePlayer BattlePlayer; // Player的战场具体表现
    internal Players WhichPlayer;
    public int ClientId;
    public bool IsInitialized = false;

    internal ClientPlayer(string username, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax, Players whichPlayer) : base(username, metalLeft, metalMax, lifeLeft, lifeMax, energyLeft, energyMax)
    {
        WhichPlayer = whichPlayer;

        BattlePlayer battlePlayer = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BattlePlayer].AllocateGameObject<BattlePlayer>(BattleManager.Instance.transform);
        battlePlayer.Initialize(this);
        BattlePlayer = battlePlayer;

        IsInitialized = true;
        SetTotalLife();
        SetTotalEnergy();
        OnMetalChanged(0);
        OnLifeChanged(0, false);
        OnEnergyChanged(0, false);
    }

    #region Metal

    protected override void OnMetalChanged(int change)
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetMetal(MetalLeft);
    }

    public void DoChangeMetal(PlayerMetalChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeMetal(request), "Co_ChangeMetal");
    }

    IEnumerator Co_ChangeMetal(PlayerMetalChangeRequest request)
    {
        if (request.metal_max != MetalMax) MetalMaxChange(request.metal_max - MetalMax);
        if (request.metal_left != MetalLeft) MetalChange(request.metal_left - MetalLeft);

        BattlePlayer.HandManager.RefreshAllCardUsable();

        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Life

    protected override void OnLifeChanged(int change, bool isOverflow)
    {
        base.OnLifeChanged(change, isOverflow);
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetLife(LifeLeft, change);
        if (change < 0)
        {
            BattlePlayer.Ship.transform.DOShakePosition(0.2f, new Vector3(0.5f, 0, 0.5f));
        }
        else if (change > 0)
        {
            BattlePlayer.Ship.ShipStyleManager.ShowShipShapeHoverForTime(0.5f);
        }
    }

    protected void SetTotalLife()
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetTotalLife(LifeMax);
    }

    public void DoChangeLife(PlayerLifeChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeLife(request), "Co_ChangeLife");
    }

    IEnumerator Co_ChangeLife(PlayerLifeChangeRequest request)
    {
        if (request.life_left != LifeLeft)
        {
            if (request.life_left - LifeLeft > 0)
            {
                Heal(request.life_left - LifeLeft);
                AudioManager.Instance.SoundPlay("sfx/OnHeal");
            }
            else if (request.life_left - LifeLeft < 0)
            {
                Damage(LifeLeft - request.life_left);
            }
        }

        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Energy

    protected override void OnEnergyChanged(int change, bool isOverflow)
    {
        base.OnEnergyChanged(change, isOverflow);
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetEnergy(EnergyLeft, change);
    }

    protected void SetTotalEnergy()
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetTotalEnergy(EnergyMax);
    }

    public void DoChangeEnergy(PlayerEnergyChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeEnergy(request), "Co_ChangeEnergy");
    }

    IEnumerator Co_ChangeEnergy(PlayerEnergyChangeRequest request)
    {
        if (request.energy_left != EnergyLeft)
        {
            if (request.energy_left - EnergyLeft > 0) AudioManager.Instance.SoundPlay("sfx/OnEnergyAdd");
            else if (request.energy_left - EnergyLeft < 0) AudioManager.Instance.SoundPlay("sfx/OnEnergyUse");
            AddEnergy(request.energy_left - EnergyLeft);
        }
        else if (request.isOverflow)
        {
            AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
        }

        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}