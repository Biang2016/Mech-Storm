public class PlayerBuffUpdateRequest : ServerRequestBase
{
    public int clientId;
    public int buffId;
    public SideEffectExecute buffSEE;

    public PlayerBuffUpdateRequest()
    {
    }

    public PlayerBuffUpdateRequest(int clientId, int buffId, SideEffectExecute buffSEE)
    {
        this.clientId = clientId;
        this.buffId = buffId;
        this.buffSEE = buffSEE;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_BUFF_UPDATE_REQUEST;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clientId);
        writer.WriteSInt32(buffId);
        buffSEE.Serialize(writer);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clientId = reader.ReadSInt32();
        buffId = reader.ReadSInt32();
        buffSEE = SideEffectExecute.Deserialize(reader);
        foreach (SideEffectBase se in buffSEE.SideEffectBases)
        {
            if (se is PlayerBuffSideEffects buff)
            {
                buff.MyBuffSEE = buffSEE;
            }
        }
    }
}