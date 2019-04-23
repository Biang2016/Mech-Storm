using System.Collections.Generic;

namespace SideEffects
{
    public class Heal : Heal_Base
    {
        public Heal()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("HealValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId).Heal(value);
            }
            else if ((TargetRange & TargetRange.Ships) == 0) // 不包含战舰
            {
                ServerBattleGroundManager.RetinueType retinueType = ServerBattleGroundManager.GetRetinueTypeByTargetRange(TargetRange);
                foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
                {
                    serverPlayer.MyBattleGroundManager.ChangeRetinuesValue(
                        ServerBattleGroundManager.RetinueValueTypes.Heal,
                        value,
                        ChoiceCount,
                        executorInfo.TargetRetinueIds,
                        TargetSelect,
                        retinueType,
                        -1
                    );
                }
            }
            else // 包含战舰
            {
                foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetShipsPlayerByTargetRange(TargetRange, player))
                {
                    serverPlayer.MyGameManager.ChangePlayerValue(ServerGameManager.PlayerValueType.Heal, value, ChoiceCount, TargetSelect, executorInfo.TargetClientIds);
                }
            }
        }
    }
}