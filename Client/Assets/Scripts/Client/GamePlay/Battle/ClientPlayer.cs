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
        SetTotalEnergy();

        BattlePlayer.MetalLifeEnergyManager.SetMetal(MetalLeft);
        BattlePlayer.MetalLifeEnergyManager.SetLife(LifeLeft, 0);
        BattlePlayer.MetalLifeEnergyManager.SetEnergy(EnergyLeft, 0);
    }

    #region Metal

    public void DoChangeMetal(PlayerMetalChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeMetal(request.metal_left), "Co_ChangeMetal");
        MetalMaxChange(request.metal_max - MetalMax);
        MetalChange(request.metal_left - MetalLeft);
        BattlePlayer.HandManager.RefreshAllCardUsable();
    }

    IEnumerator Co_ChangeMetal(int metalLeft)
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetMetal(metalLeft);
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Life

    protected void SetTotalLife()
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetTotalLife(LifeMax);
    }

    public void DoChangeLife(PlayerLifeChangeRequest request)
    {
        if (request.Life_left - LifeLeft > 0)
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeLife(LifeLeft, request.Life_left - LifeLeft, request.IsOverflow), "Co_ChangeLife");
            Heal(request.Life_left - LifeLeft);
        }
        else if (request.Life_left - LifeLeft < 0)
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeLife(LifeLeft, request.Life_left - LifeLeft, request.IsOverflow), "Co_ChangeLife");
            Damage(LifeLeft - request.Life_left);
        }
        else
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeLife(LifeLeft, 0, request.IsOverflow), "Co_ChangeLife");
        }
    }

    IEnumerator Co_ChangeLife(int lifeLeft, int change, bool isOverflow)
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetLife(lifeLeft + change, change);
        if (change < 0)
        {
            BattlePlayer.Ship.ResetPosition();
            BattlePlayer.Ship.transform.DOShakePosition(0.2f, new Vector3(0.5f, 0, 0.5f));
            AudioManager.Instance.SoundPlay("sfx/OnHitShip");
            AudioManager.Instance.SoundPlay("sfx/OnHitShipDuuu");
        }
        else if (change > 0)
        {
            BattlePlayer.Ship.ShipStyleManager.ShowShipShapeHoverForTime(0.5f * BattleEffectsManager.AnimationSpeed);
            AudioManager.Instance.SoundPlay("sfx/OnHeal");
        }
        else if (isOverflow)
        {
            BattlePlayer.MetalLifeEnergyManager.LifeOverflowJump();
            AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
        }

        yield return new WaitForSeconds(0.1f * BattleEffectsManager.AnimationSpeed);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Energy

    protected void SetTotalEnergy()
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetTotalEnergy(EnergyMax);
    }

    public void DoChangeEnergy(PlayerEnergyChangeRequest request)
    {
        if (request.Energy_left - EnergyLeft > 0)
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeEnergy(EnergyLeft, request.Energy_left - EnergyLeft, request.IsOverflow), "Co_ChangeEnergy");
            AddEnergy(request.Energy_left - EnergyLeft);
        }
        else if (request.Energy_left - EnergyLeft < 0)
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeEnergy(EnergyLeft, request.Energy_left - EnergyLeft, request.IsOverflow), "Co_ChangeEnergy");
            UseEnergy(EnergyLeft - request.Energy_left);
        }
        else
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeEnergy(EnergyLeft, 0, request.IsOverflow), "Co_ChangeEnergy");
        }
    }

    IEnumerator Co_ChangeEnergy(int energyLeft, int change, bool isOverflow)
    {
        if (IsInitialized) BattlePlayer.MetalLifeEnergyManager.SetEnergy(energyLeft + change, change);
        if (change > 0)
        {
            AudioManager.Instance.SoundPlay("sfx/OnEnergyAdd");
        }
        else if (change < 0)
        {
            AudioManager.Instance.SoundPlay("sfx/OnEnergyUse");
        }
        else if (isOverflow)
        {
            BattlePlayer.MetalLifeEnergyManager.EnergyOverflowJump();
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