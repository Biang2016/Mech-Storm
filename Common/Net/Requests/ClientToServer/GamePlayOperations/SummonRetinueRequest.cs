using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int battleGroundIndex;

    public SummonRetinueRequest()
    {

    }

    public SummonRetinueRequest(int clientId,  int handCardInstanceId, int battleGroundIndex):base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.battleGroundIndex = battleGroundIndex;
    }
    public override int GetProtocol()
    {
        return NetProtocols.SUMMON_RETINUE_REQUEST;
    }

	public override string GetProtocolName()
	{
        return "SUMMON_RETINUE_REQUEST";
	}

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardInstanceId]=" + handCardInstanceId;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        return log;
    }
}