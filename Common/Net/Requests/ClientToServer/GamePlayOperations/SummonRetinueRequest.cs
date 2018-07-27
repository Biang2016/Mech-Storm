using System.Collections;
using System.Collections.Generic;

public class SummonRetinueRequest : ClientRequestBase
{
    public int handCardIndex;
    public int battleGroundIndex;

    public SummonRetinueRequest()
    {

    }

    public SummonRetinueRequest(int clientId,  int handCardIndex, int battleGroundIndex):base(clientId)
    {
        this.handCardIndex = handCardIndex;
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
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardIndex]=" + handCardIndex;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        return log;
    }
}