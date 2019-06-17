using System.Collections.Generic;

public class UseSpellCardToEquipRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public List<int> targetEquipIds;

    public UseSpellCardToEquipRequest()
    {
    }

    public UseSpellCardToEquipRequest(int clientId, int handCardInstanceId, List<int> targetEquipIds) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.targetEquipIds = targetEquipIds ?? new List<int>();
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_EQUIP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetEquipIds.Count);
        foreach (int id in targetEquipIds)
        {
            writer.WriteSInt32(id);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();

        targetEquipIds = new List<int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int id = reader.ReadSInt32();
            targetEquipIds.Add(id);
        }
    }
}