using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Boss
{
    public int BossID;
    public string Name;
    public string BuildName;
    public int PicID;

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(BossID);
        writer.WriteString8(Name);
        writer.WriteString8(BuildName);
        writer.WriteSInt32(PicID);
    }

    public static Boss Deserialize(DataStream reader)
    {
        Boss newBoss = new Boss();
        newBoss.BossID = reader.ReadSInt32();
        newBoss.Name = reader.ReadString8();
        newBoss.BuildName = reader.ReadString8();
        newBoss.PicID = reader.ReadSInt32();
        return newBoss;
    }

    public string DeserializeLog()
    {
        string log = "";
        log += " [BossID]=" + BossID;
        log += " [Name]=" + Name;
        log += " [BuildName]=" + BuildName;
        log += " [PicID]=" + PicID;
        return log;
    }
}