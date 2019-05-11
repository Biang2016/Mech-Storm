using System.Collections.Generic;

namespace SideEffects
{
    public class RunAway : RunAway_Base
    {
        public RunAway()
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
                    int cardInstanceID = mech.OriginCardInstanceId;
                    int cardID = mech.CardInfo.CardID;
                    mech.OriginCardInstanceId = -1;

                    int weaponCardInstanceID = -1;
                    int weaponCardID = -1;
                    int shieldCardInstanceID = -1;
                    int shieldCardID = -1;
                    int packCardInstanceID = -1;
                    int packCardID = -1;
                    int maCardInstanceID = -1;
                    int maCardID = -1;

                    if (mech.M_Weapon != null)
                    {
                        weaponCardInstanceID = mech.M_Weapon.OriginCardInstanceId;
                        weaponCardID = mech.M_Weapon.CardInfo.CardID;
                        mech.M_Weapon.OriginCardInstanceId = -1;
                    }

                    if (mech.M_Shield != null)
                    {
                        shieldCardInstanceID = mech.M_Shield.OriginCardInstanceId;
                        shieldCardID = mech.M_Shield.CardInfo.CardID;
                        mech.M_Shield.OriginCardInstanceId = -1;
                    }

                    if (mech.M_Pack != null)
                    {
                        packCardInstanceID = mech.M_Pack.OriginCardInstanceId;
                        packCardID = mech.M_Pack.CardInfo.CardID;
                        mech.M_Pack.OriginCardInstanceId = -1;
                    }

                    if (mech.M_MA != null)
                    {
                        maCardInstanceID = mech.M_MA.OriginCardInstanceId;
                        maCardID = mech.M_MA.CardInfo.CardID;
                        mech.M_MA.OriginCardInstanceId = -1;
                    }

                    player.MyGameManager.KillMechs(new List<int> {mechId});
                    player.MyHandManager.GetACardByID(cardID, cardInstanceID);
                    if (weaponCardInstanceID != -1) player.MyHandManager.GetACardByID(weaponCardID, weaponCardInstanceID);
                    if (shieldCardInstanceID != -1) player.MyHandManager.GetACardByID(shieldCardID, shieldCardInstanceID);
                    if (packCardInstanceID != -1) player.MyHandManager.GetACardByID(packCardID, packCardInstanceID);
                    if (maCardInstanceID != -1) player.MyHandManager.GetACardByID(maCardID, maCardInstanceID);
                }
            }
        }
    }
}