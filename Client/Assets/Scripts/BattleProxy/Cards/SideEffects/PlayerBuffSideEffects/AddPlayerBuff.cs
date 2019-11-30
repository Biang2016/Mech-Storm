namespace SideEffects
{
    public class AddPlayerBuff : AddPlayerBuff_Base
    {
        public AddPlayerBuff()
        {
        }

        public override bool Execute(ExecutorInfo executorInfo)
        {
            if (!base.Execute(executorInfo)) return false;
            BattlePlayer player = (BattlePlayer) Player;

            AttachedBuffSEE.M_ExecutorInfo = new ExecutorInfo(clientId: player.ClientId, sideEffectExecutorID: AttachedBuffSEE.ID, isPlayerBuff: true);
            foreach (SideEffectBase se in AttachedBuffSEE.SideEffectBases)
            {
                se.Player = player;
                se.M_SideEffectExecute = AttachedBuffSEE;
            }

            if (AttachedBuffSEE.M_ExecuteSetting is ScriptExecuteSettingBase sesb)
            {
                sesb.Player = player;
            }

            player.UpdatePlayerBuff(AttachedBuffSEE, true);
            return true;
        }
    }
}