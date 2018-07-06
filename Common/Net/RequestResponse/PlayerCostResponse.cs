using System.Collections;
using System.Collections.Generic;

public class PlayerCostResponse : Response
{
    public int ClinetId;
    public int Sign; //+2加满，+1增加，-1减少，-2扣光
    public int AddCost;

    public override int GetProtocol()
    {
        return NetProtocols.PLAYER_COST_CHANGE;
    }

    public override string GetProtocolName()
    {
        return GetType().FullName;
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        ClinetId = reader.ReadSInt32();
        Sign = reader.ReadSInt16();
        AddCost = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += "[ClinetId]" + ClinetId;
        log += "[Sign]" + Sign;
        log += "[AddCost]" + AddCost;
        return log;
    }
}