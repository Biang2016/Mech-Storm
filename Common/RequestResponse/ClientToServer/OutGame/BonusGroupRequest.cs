public class BonusGroupRequest : ClientRequestBase
{
    public BonusGroup BonusGroup;

    public BonusGroupRequest() : base()
    {
    }

    public BonusGroupRequest(int clientId, BonusGroup bonusGroup) : base(clientId)
    {
        BonusGroup = bonusGroup;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.BONUSGROUP_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        BonusGroup.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BonusGroup = BonusGroup.Deserialize(reader);
    }
}