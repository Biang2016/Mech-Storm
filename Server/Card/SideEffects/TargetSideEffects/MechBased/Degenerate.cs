﻿using System;

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
                foreach (int mechId in executorInfo.TargetMechIds)
                {
                    ServerModuleMech mech = player.MyBattleGroundManager.GetMech(mechId);
                    if (mech.M_Weapon != null)
                    {
                        mech.M_Weapon = null;
                        AddRandomAttrToMech(mech);
                    }

                    if (mech.M_Shield != null)
                    {
                        mech.M_Shield = null;
                        AddRandomAttrToMech(mech);
                    }

                    if (mech.M_Pack != null)
                    {
                        mech.M_Pack = null;
                        AddRandomAttrToMech(mech);
                    }

                    if (mech.M_MA != null)
                    {
                        mech.M_MA = null;
                        AddRandomAttrToMech(mech);
                    }
                }
            }
        }

        private void AddRandomAttrToMech(ServerModuleMech mech)
        {
            Random rd = new Random();
            int random = rd.Next(0, 4);
            switch (random)
            {
                case 0:
                {
                    mech.CardInfo.BattleInfo.BasicAttack += 1;
                    mech.M_MechAttack += 1;
                    break;
                }
                case 1:
                {
                    mech.CardInfo.BattleInfo.BasicArmor += 5;
                    mech.M_MechArmor += 5;
                    break;
                }
                case 2:
                {
                    mech.CardInfo.BattleInfo.BasicShield += 2;
                    mech.M_MechShield += 2;
                    break;
                }
                case 3:
                {
                    mech.CardInfo.LifeInfo.TotalLife += 3;
                    mech.CardInfo.LifeInfo.Life += 3;
                    mech.AddLife(3);
                    break;
                }
            }

            MechCardInfoSyncRequest request = new MechCardInfoSyncRequest(mech.ServerPlayer.ClientId, mech.M_MechID, mech.CardInfo.Clone());
            mech.ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
        }
    }
}