using System.Collections.Generic;

namespace SideEffects
{
    public class AddLife : AddLife_Base
    {
        public AddLife()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            int value = M_SideEffectParam.GetParam_MultipliedInt("LifeValue");
            if (TargetRange == TargetRange.Self) // 对自身
            {
                player.MyBattleGroundManager.GetRetinue(executorInfo.RetinueId).AddLife(value);
            }
            else if ((TargetRange & TargetRange.Ships) == 0) // 不包含战舰
            {
                ServerBattleGroundManager.RetinueType retinueType = ServerBattleGroundManager.GetRetinueTypeByTargetRange(TargetRange);

                foreach (ServerPlayer serverPlayer in ServerBattleGroundManager.GetMechsPlayerByTargetRange(TargetRange, player))
                {
                    serverPlayer.MyBattleGroundManager.ChangeRetinuesValue(
                        ServerBattleGroundManager.RetinueValueTypes.Life,
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
                    serverPlayer.MyGameManager.ChangePlayerValue(ServerGameManager.PlayerValueType.AddLife, value, ChoiceCount, TargetSelect, executorInfo.TargetClientIds);
                }
            }
        }
    }
}