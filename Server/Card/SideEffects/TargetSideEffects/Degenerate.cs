using System;

namespace SideEffects
{
    public class Degenerate : Degenerate_Base
    {
        public Degenerate()
        {
        }

        public override void Execute(ExecuterInfo executerInfo)
        {
            ServerPlayer player = (ServerPlayer) Player;
            switch (M_TargetRange)
            {
                case TargetRange.SelfHeroes:
                    ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(executerInfo.TargetRetinueId);
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

                    break;
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
                    retinue.M_RetinueTotalLife += 3;
                    retinue.M_RetinueLeftLife += 3;
                    break;
                }
            }

            RetinueCardInfoSyncRequest request = new RetinueCardInfoSyncRequest(retinue.ServerPlayer.ClientId, retinue.M_RetinueID, retinue.CardInfo.Clone());
            retinue.ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
        }
    }
}