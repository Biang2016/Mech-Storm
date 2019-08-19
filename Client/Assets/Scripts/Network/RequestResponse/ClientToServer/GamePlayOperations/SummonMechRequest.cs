using System.Collections.Generic;

public class SummonMechRequest : ClientRequestBase
{
    public int handCardInstanceId;
    public int battleGroundIndex;
    public int clientMechTempId; //召唤的随从的客户端临时ID号，用于预召唤随从的匹配
    public List<int> targetMechIds;
    public List<bool> isTargetMechIdTempIds; //目标ID是否是临时ID  ,用于防止用户操作过快（当上一个操作也是预召唤随从的时候，其随从ID还没有从服务器获得，此时对该随从进行指向操作可能引发错误，因此需要一个bool表来记录目标随从是否是临时随从）

    public SummonMechRequest()
    {
    }

    public SummonMechRequest(int clientId, int handCardInstanceId, int battleGroundIndex, int clientMechTempId, List<int> targetMechIds, List<bool> isTargetMechIdTempIds) : base(clientId)
    {
        this.handCardInstanceId = handCardInstanceId;
        this.battleGroundIndex = battleGroundIndex;
        this.clientMechTempId = clientMechTempId;
        this.targetMechIds = targetMechIds ?? new List<int>();
        this.isTargetMechIdTempIds = isTargetMechIdTempIds ?? new List<bool>();
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SUMMON_MECH_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardInstanceId);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(clientMechTempId);
        writer.WriteSInt32(targetMechIds.Count);
        foreach (int id in targetMechIds)
        {
            writer.WriteSInt32(id);
        }

        writer.WriteSInt32(isTargetMechIdTempIds.Count);
        foreach (bool isTemp in isTargetMechIdTempIds)
        {
            writer.WriteByte((byte) (isTemp ? 0x01 : 0x00));
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardInstanceId = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        clientMechTempId = reader.ReadSInt32();
        targetMechIds = new List<int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int id = reader.ReadSInt32();
            targetMechIds.Add(id);
        }

        isTargetMechIdTempIds = new List<bool>();
        count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            bool isTemp = reader.ReadByte() == 0x01;
            isTargetMechIdTempIds.Add(isTemp);
        }
    }
}