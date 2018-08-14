using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideEffects
{
    public class DamageSomeRetinue : DamageSomeRetinue_Base
    {
        public DamageSomeRetinue()
        {
        }

        public override void Excute(object Player)
        {
            ClientPlayer cp = (ClientPlayer) Player;
            cp.MyBattleGroundManager.DamageSomeRetinue(TargetRetinueId, Value);
        }
    }
}