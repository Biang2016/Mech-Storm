using System.Collections;
using System.Collections.Generic;

public class PlayerResponse : Response
{
    public int ClinetId;
    public int CostLeft;
    public int CostMax;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ClinetId = reader.ReadSInt32();
        CostLeft = reader.ReadSInt32();
        CostMax = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[ClinetId]" + ClinetId;
        log += "[CostLeft]" + CostLeft;
        log += "[CostMax]" + CostMax;
        return log;
    }
}