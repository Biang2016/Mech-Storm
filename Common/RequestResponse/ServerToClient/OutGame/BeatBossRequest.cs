using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BeatBossRequest:ServerRequestBase
{
    public int LevelID;
    public int BossID;
    //Other 奖励

    public BeatBossRequest()
    {
    }

    public BeatBossRequest(int levelID, int bossID)
    {
        LevelID = levelID;
        BossID = bossID;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BEAT_BOSS_REQUSET;
    }

    public override string GetProtocolName()
    {
        return "BEAT_BOSS_REQUSET";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelID);
        writer.WriteSInt32(BossID);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        LevelID = reader.ReadSInt32();
        BossID = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [LevelID]=" + LevelID;
        log += " [BossID]=" + BossID;
        return log;
    }
}