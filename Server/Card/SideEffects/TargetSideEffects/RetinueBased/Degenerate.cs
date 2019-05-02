using System;

namespace SideEffects
{
    public class Degenerate : Degenerate_Base
    {
        public Degenerate()
        {
        }

        public override void Execute(ExecutorInfo executorInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            if ((TargetRange & TargetRange.Mechs) != 0)
            {
                foreach (int retinueId in executorInfo.TargetRetinueIds)
                {
                    ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(retinueId);
                    if (retinue.M_Weapon != null)
                    {
                        retinue.M_Weapon = null;
                        AddRandomAttrToRetinue(retinue);
                    }

                    if (retinue.M_Shield != null)
                    {
                        retinue.M_Shield = null;
                        AddRandomAttrToRetinue(retinue);
                    }

                    if (retinue.M_Pack != null)
                    {
                        retinue.M_Pack = null;
                        AddRandomAttrToRetinue(retinue);
                    }

                    if (retinue.M_MA != null)
                    {
                        retinue.M_MA = null;
                        AddRandomAttrToRetinue(retinue);
                    }
                }
            }
        }

        private void AddRandomAttrToRetinue(ServerModuleRetinue retinue)
        {
            Random rd = new Random();
            int random = rd.Next(0, 4);
            switch (random)
            {
                case 0:
                {
                    retinue.CardInfo.BattleInfo.BasicAttack += 1;
                    retinue.M_RetinueAttack += 1;
                    break;
                }
                case 1:
                {
                    retinue.CardInfo.BattleInfo.BasicArmor += 5;
                    retinue.M_RetinueArmor += 5;
                    break;
                }
                case 2:
                {
                    retinue.CardInfo.BattleInfo.BasicShield += 2;
                    retinue.M_RetinueShield += 2;
                    break;
                }
                case 3:
                {
                    retinue.CardInfo.LifeInfo.TotalLife += 3;
                    retinue.CardInfo.LifeInfo.Life += 3;
                    retinue.AddLife(3);
                    break;
                }
            }

            RetinueCardInfoSyncRequest request = new RetinueCardInfoSyncRequest(retinue.ServerPlayer.ClientId, retinue.M_RetinueID, retinue.CardInfo.Clone());
            retinue.ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
        }
    }
}