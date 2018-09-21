public class PlayerLifeChangeRequest : ServerRequestBase
{
    public int clinetId;
    public LifeChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int addLife_left;
    public int addLife_max;

    public PlayerLifeChangeRequest()
    {
    }

    public PlayerLifeChangeRequest(int clinetId, LifeChangeFlag change, int addLife_left = 0, int addLife_max = 0)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.addLife_left = addLife_left;
        this.addLife_max = addLife_max;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_PLAYER_LIFE_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_LIFE_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteByte((byte) change);
        if (change == LifeChangeFlag.Both)
        {
            writer.WriteSInt32(addLife_left);
            writer.WriteSInt32(addLife_max);
        }
        else if (change == LifeChangeFlag.Left)
        {
            writer.WriteSInt32(addLife_left);
        }
        else if (change == LifeChangeFlag.Max)
        {
            writer.WriteSInt32(addLife_max);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (LifeChangeFlag) reader.ReadByte();
        if (change == LifeChangeFlag.Both)
        {
            addLife_left = reader.ReadSInt32();
            addLife_max = reader.ReadSInt32();
        }
        else if (change == LifeChangeFlag.Left)
        {
            addLife_left = reader.ReadSInt32();
        }
        else if (change == LifeChangeFlag.Max)
        {
            addLife_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [change]=" + change;
        log += " [addLife_left]=" + addLife_left;
        log += " [addLife_max]=" + addLife_max;
        return log;
    }

    public enum LifeChangeFlag
    {
        Both = 0x00,
        Left = 0x01,
        Max = 0x02
    }
}