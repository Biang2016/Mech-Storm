using System.Collections.Generic;

public class BattleGroundRemoveRetinueRequest : ServerRequestBase
{
    public List<int> retinueIds = new List<int>();

    public BattleGroundRemoveRetinueRequest()
    {
    }

    public BattleGroundRemoveRetinueRequest(List<int> retinueIds)
    {
        foreach (int retinueId in retinueIds)
        {
            this.retinueIds.Add(retinueId);
        }
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_BATTLEGROUND_REMOVE_RETINUE;
    }

    public override string GetProtocolName()
    {
        return "SE_BATTLEGROUND_REMOVE_RETINUE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(retinueIds.Count);
        foreach (int retinueId in retinueIds)
        {
            writer.WriteSInt32(retinueId);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int retinueId = reader.ReadSInt32();
            retinueIds.Add(retinueId);
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [retinueInfos]= ";
        foreach (int retinueId in retinueIds)
        {
            log += retinueId + ", ";
        }

        return log;
    }
}