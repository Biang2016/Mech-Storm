using System.Collections.Generic;

public class UseSpellCardToShipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public List<int> targetClientIds;

    public UseSpellCardToShipRequest()
    {
    }

    public UseSpellCardToShipRequest(int clientId, int handCardInstanceId, List<int> targetClientIds) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.targetClientIds = targetClientIds ?? new List<int>();
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_SHIP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetClientIds.Count);
        foreach (int id in targetClientIds)
        {
            writer.WriteSInt32(id);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        targetClientIds = new List<int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int id = reader.ReadSInt32();
            targetClientIds.Add(id);
        }
    }
}