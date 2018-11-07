using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NextLevelBossesRequest : ServerRequestBase
{
    public int LevelID;
    public List<int> NextLevelBossPicIDs = new List<int>();

    public NextLevelBossesRequest()
    {
    }

    public NextLevelBossesRequest(int levelID, List<int> nextLevelBossPicIDs)
    {
        LevelID = levelID;
        NextLevelBossPicIDs = nextLevelBossPicIDs;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.NEXT_LEVEL_BOSSINFO_REQUSET;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(NextLevelBossPicIDs.Count);
        foreach (int picID in NextLevelBossPicIDs)
        {
            writer.WriteSInt32(picID);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        LevelID = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            NextLevelBossPicIDs.Add(reader.ReadSInt32());
        }
    }
}