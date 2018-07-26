using System.Collections;
using System.Collections.Generic;

public class BattleGroundRemoveRetinueRequest : ServerRequestBase
{
    public List<RetinuePlaceInfo> retinueInfos = new List<RetinuePlaceInfo>();

    public BattleGroundRemoveRetinueRequest()
    {
    }

    public BattleGroundRemoveRetinueRequest(List<RetinuePlaceInfo> retinueInfos)
    {
        this.retinueInfos = retinueInfos;
    }

    public override int GetProtocol()
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
        writer.WriteSInt32(retinueInfos.Count);
        foreach (RetinuePlaceInfo info in retinueInfos)
        {
            writer.WriteSInt32(info.clientId);
            writer.WriteSInt32(info.retinuePlaceIndex);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int clientId = reader.ReadSInt32();
            int retinuePlaceIndex = reader.ReadSInt32();
            RetinuePlaceInfo info = new RetinuePlaceInfo(clientId, retinuePlaceIndex);
            retinueInfos.Add(info);
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [retinueInfos]= cid->retinuePlaceIndex: ";
        foreach (RetinuePlaceInfo info in retinueInfos)
        {
            log += info.clientId + "->" + info.retinuePlaceIndex + ", ";
        }
        return log;
    }
}