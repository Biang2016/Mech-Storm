using System.Collections;
using UnityEngine;

public class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal MetalLifeEnergyManager MyMetalLifeEnergyManager; //Metal、Energy、Life条的管理器
    internal PlayerBuffManager MyPlayerBuffManager; //Buff的管理器
    internal CoolDownCardManager MyPlayerCoolDownCardManager; //Buff的管理器
    internal BoardAreaTypes MyHandArea; //卡牌所属的手部区
    internal Players WhichPlayer;
    public int ClientId;
    public bool IsInitialized = false;

    internal ClientPlayer(string username, int metalLeft, int metalMax, int lifeLeft, int lifeMax, int energyLeft, int energyMax, Players whichPlayer) : base(username, metalLeft, metalMax, lifeLeft, lifeMax, energyLeft, energyMax)
    {
        WhichPlayer = whichPlayer;
        MyHandArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        MyBattleGroundArea = whichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
        MyHandManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfHandManager : GameBoardManager.Instance.EnemyHandManager;
        MyBattleGroundManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfBattleGroundManager : GameBoardManager.Instance.EnemyBattleGroundManager;
        MyMetalLifeEnergyManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfMetalLifeEnergyManager : GameBoardManager.Instance.EnemyMetalLifeEnergyManager;
        MyPlayerBuffManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfPlayerBuffManager : GameBoardManager.Instance.EnemyPlayerBuffManager;
        MyPlayerCoolDownCardManager = whichPlayer == Players.Self ? GameBoardManager.Instance.SelfPlayerCoolDownCardManager : GameBoardManager.Instance.EnemyPlayerCoolDownCardManager;
        MyHandManager.ClientPlayer = this;
        MyMetalLifeEnergyManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
        MyPlayerBuffManager.ClientPlayer = this;
        MyPlayerCoolDownCardManager.ClientPlayer = this;
        IsInitialized = true;
        SetTotalLife();
        SetTotalEnergy();
        OnMetalChanged(0);
        OnLifeChanged(0);
        OnEnergyChanged(0);
    }

    #region Metal

    protected override void OnMetalChanged(int change)
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetMetal(MetalLeft);
    }

    public void DoChangeMetal(PlayerMetalChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeMetal(request), "Co_ChangeMetal");
    }

    IEnumerator Co_ChangeMetal(PlayerMetalChangeRequest request)
    {
        if (request.metal_max != MetalMax) AddMetalMax(request.metal_max - MetalMax);
        if (request.metal_left != MetalLeft) AddMetal(request.metal_left - MetalLeft);

        MyHandManager.RefreshAllCardUsable();

        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Life

    protected override void OnLifeChanged(int change)
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetLife(LifeLeft, change);
    }

    protected void SetTotalLife()
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetTotalLife(LifeMax);
    }

    public void DoChangeLife(PlayerLifeChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeLife(request), "Co_ChangeLife");
    }

    IEnumerator Co_ChangeLife(PlayerLifeChangeRequest request)
    {
        if (request.life_left != LifeLeft)
        {
            if (request.life_left - LifeLeft > 0) AudioManager.Instance.SoundPlay("sfx/OnHeal");
            AddLife(request.life_left - LifeLeft);
        }

        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Energy

    protected override void OnEnergyChanged(int change)
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetEnergy(EnergyLeft, change);
    }

    protected void SetTotalEnergy()
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetTotalEnergy(EnergyMax);
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
            AudioManager.Instance.SoundPlay("sfx/OnSelectRetinueFalse");
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