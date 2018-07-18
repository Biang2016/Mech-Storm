using System.Collections;
using System.Collections.Generic;

public class BattleGroundRemoveRetinueRequest : ServerRequestBase
{
    public int clientId;
    public int battleGroundIndex;

    public BattleGroundRemoveRetinueRequest()
    {
    }

    public BattleGroundRemoveRetinueRequest(int clientId, int battleGroundIndex)
    {
        this.clientId = clientId;
        this.battleGroundIndex = battleGroundIndex;
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
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(battleGroundIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clientId]=" + clientId;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        return log;
    }
}