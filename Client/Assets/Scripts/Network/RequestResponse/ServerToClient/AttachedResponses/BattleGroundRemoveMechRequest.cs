using System.Collections.Generic;

public class BattleGroundRemoveMechRequest : ServerRequestBase
{
    public List<int> mechIds = new List<int>();

    public BattleGroundRemoveMechRequest()
    {
    }

    public BattleGroundRemoveMechRequest(List<int> mechIds)
    {
        foreach (int mechId in mechIds)
        {
            this.mechIds.Add(mechId);
        }
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_BATTLEGROUND_REMOVE_MECH;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(mechIds.Count);
        foreach (int mechId in mechIds)
        {
            writer.WriteSInt32(mechId);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int mechId = reader.ReadSInt32();
            mechIds.Add(mechId);
        }
    }
}