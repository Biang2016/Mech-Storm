using System;
using System.Collections.Generic;

public class UseSpellCardToMechRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public List<(int, bool)> targetMechIds;

    public UseSpellCardToMechRequest()
    {
    }

    public UseSpellCardToMechRequest(int clientId, int handCardInstanceId, List<(int, bool)> targetMechIds) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.targetMechIds = targetMechIds ?? new List<ValueTuple<int, bool>>();
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.USE_SPELLCARD_TO_MECH_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(targetMechIds.Count);
        foreach (ValueTuple<int, bool> var in targetMechIds)
        {
            writer.WriteSInt32(var.Item1);
            writer.WriteByte((byte) (var.Item2 ? 0x01 : 0x00));
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int id = reader.ReadSInt32();
            bool isTemp = reader.ReadByte() == 0x01;
            targetMechIds.Add((id, isTemp));
        }
    }
}