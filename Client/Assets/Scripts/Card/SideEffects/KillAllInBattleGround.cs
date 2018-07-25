using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

internal class KillAllInBattleGround : KillAllInBattleGround_Base
{
    public KillAllInBattleGround()
    {
    }

    public override SideEffectBase Clone()
    {
        Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
        string type = GetType().ToString();
        SideEffectBase se = (SideEffectBase) assembly.CreateInstance(type);
        se.SideEffectID = SideEffectID;
        se.Name = Name;
        se.Desc = Desc;

        //KillAllInBattleGround se = (KillAllInBattleGround)base.Clone();
        //se.Info = Info;
        return se;
    }

    public override void RefreshDesc()
    {
        Desc = String.Format(Desc, Info.WhoseBattleGround);
    }

    public override void Excute(object Player)
    {
        ClientPlayer player = (ClientPlayer)Player;
        switch (Info.WhoseBattleGround)
        {
            case "我方":


                break;
            case "敌方":


                break;
            case "":


                break;
        }
    }
}