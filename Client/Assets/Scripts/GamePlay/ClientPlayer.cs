using System.Collections;
using System.Collections.Generic;

public class ClientPlayer : Player
{
    public BoardAreaTypes MyBattleGroundArea; //卡牌所属方的战场区
    public HandManager MyHandManager; //卡牌所属的手部区管理器
    internal BattleGroundManager MyBattleGroundManager; //卡牌所属方的战场区域管理器
    internal MetalLifeEnergyManager MyMetalLifeEnergyManager; //Metal、Energy、Life条的管理器
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
        MyHandManager.ClientPlayer = this;
        MyMetalLifeEnergyManager.ClientPlayer = this;
        MyBattleGroundManager.ClientPlayer = this;
        IsInitialized = true;
        SetTotalLife();
        SetTotalEnergy();
        OnMetalChanged();
        OnLifeChanged();
        OnEnergyChanged();
    }

    #region Metal

    protected override void OnMetalChanged()
    {
        if (IsInitialized)
        {
            MyMetalLifeEnergyManager.SetMetal(MetalLeft);
        }
    }

    public void DoChangeMetal(PlayerMetalChangeRequest request)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ChangeMetal(request), "Co_ChangeMetal");
    }

    IEnumerator Co_ChangeMetal(PlayerMetalChangeRequest request)
    {
        if (request.change == PlayerMetalChangeRequest.MetalChangeFlag.Both)
        {
            AddMetal(request.addMetal_left);
            AddMetalMax(request.addMetal_max);
        }
        else if (request.change == PlayerMetalChangeRequest.MetalChangeFlag.Left)
        {
            AddMetal(request.addMetal_left);
        }
        else if (request.change == PlayerMetalChangeRequest.MetalChangeFlag.Max)
        {
            AddMetalMax(request.addMetal_max);
        }

        MyHandManager.RefreshAllCardUsable();

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Life

    protected override void OnLifeChanged()
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetLife(LifeLeft);
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
        if (request.change == PlayerLifeChangeRequest.LifeChangeFlag.Left)
        {
            AddLife(request.addLife_left);
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Energy

    protected override void OnEnergyChanged()
    {
        if (IsInitialized) MyMetalLifeEnergyManager.SetEnergy(EnergyLeft);
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
        if (request.change == PlayerEnergyChangeRequest.EnergyChangeFlag.Left)
        {
            AddEnergy(request.addEnergy_left);
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion
}

internal enum Players
{
    Self = 0,
    Enemy = 1
}