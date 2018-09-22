using System.Collections.Generic;

public class RetinueDieRequest : ServerRequestBase
{
    public List<int> retinueIds = new List<int>();

    public RetinueDieRequest()
    {
    }

    public RetinueDieRequest(List<int> retinueIds)
    {
        foreach (int retinueId in retinueIds)
        {
            this.retinueIds.Add(retinueId);
        }
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_RETINUE_DIE;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_DIE";
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