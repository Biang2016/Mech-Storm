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
                foreach (int retinueId in executorInfo.TargetRetinueIds)
                {
                    ServerModuleRetinue retinue = player.MyBattleGroundManager.GetRetinue(retinueId);
                    int cardInstanceID = retinue.OriginCardInstanceId;
                    int cardID = retinue.CardInfo.CardID;
                    retinue.OriginCardInstanceId = -1;

                    int weaponCardInstanceID = -1;
                    int weaponCardID = -1;
                    int shieldCardInstanceID = -1;
                    int shieldCardID = -1;
                    int packCardInstanceID = -1;
                    int packCardID = -1;
                    int maCardInstanceID = -1;
                    int maCardID = -1;

                    if (retinue.M_Weapon != null)
                    {
                        weaponCardInstanceID = retinue.M_Weapon.OriginCardInstanceId;
                        weaponCardID = retinue.M_Weapon.CardInfo.CardID;
                        retinue.M_Weapon.OriginCardInstanceId = -1;
                    }

                    if (retinue.M_Shield != null)
                    {
                        shieldCardInstanceID = retinue.M_Shield.OriginCardInstanceId;
                        shieldCardID = retinue.M_Shield.CardInfo.CardID;
                        retinue.M_Shield.OriginCardInstanceId = -1;
                    }

                    if (retinue.M_Pack != null)
                    {
                        packCardInstanceID = retinue.M_Pack.OriginCardInstanceId;
                        packCardID = retinue.M_Pack.CardInfo.CardID;
                        retinue.M_Pack.OriginCardInstanceId = -1;
                    }

                    if (retinue.M_MA != null)
                    {
                        maCardInstanceID = retinue.M_MA.OriginCardInstanceId;
                        maCardID = retinue.M_MA.CardInfo.CardID;
                        retinue.M_MA.OriginCardInstanceId = -1;
                    }

                    player.MyGameManager.KillRetinues(new List<int> {retinueId});
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