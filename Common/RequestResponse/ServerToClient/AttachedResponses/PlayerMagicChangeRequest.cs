public class PlayerMagicChangeRequest : ServerRequestBase
{
    public int clinetId;
    public MagicChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int addMagic_left;
    public int addMagic_max;

    public PlayerMagicChangeRequest()
    {
    }

    public PlayerMagicChangeRequest(int clinetId, MagicChangeFlag change, int addMagic_left = 0, int addMagic_max = 0)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.addMagic_left = addMagic_left;
        this.addMagic_max = addMagic_max;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_PLAYER_MAGIC_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_MAGIC_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteByte((byte) change);
        if (change == MagicChangeFlag.Both)
        {
            writer.WriteSInt32(addMagic_left);
            writer.WriteSInt32(addMagic_max);
        }
        else if (change == MagicChangeFlag.Left)
        {
            writer.WriteSInt32(addMagic_left);
        }
        else if (change == MagicChangeFlag.Max)
        {
            writer.WriteSInt32(addMagic_max);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (MagicChangeFlag) reader.ReadByte();
        if (change == MagicChangeFlag.Both)
        {
            addMagic_left = reader.ReadSInt32();
            addMagic_max = reader.ReadSInt32();
        }
        else if (change == MagicChangeFlag.Left)
        {
            addMagic_left = reader.ReadSInt32();
        }
        else if (change == MagicChangeFlag.Max)
        {
            addMagic_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [change]=" + change;
        log += " [addMagic_left]=" + addMagic_left;
        log += " [addMagic_max]=" + addMagic_max;
        return log;
    }

    public enum MagicChangeFlag
    {
        Both = 0x00,
        Left = 0x01,
        Max = 0x02
    }
}