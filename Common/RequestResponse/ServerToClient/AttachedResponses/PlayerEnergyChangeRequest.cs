public class PlayerEnergyChangeRequest : ServerRequestBase
{
    public int clinetId;
    public EnergyChangeFlag change; //0x00为都改变，0x01为left改变，0x02为max改变
    public int addEnergy_left;
    public int addEnergy_max;

    public PlayerEnergyChangeRequest()
    {
    }

    public PlayerEnergyChangeRequest(int clinetId, EnergyChangeFlag change, int addEnergy_left = 0, int addEnergy_max = 0)
    {
        this.clinetId = clinetId;
        this.change = change;
        this.addEnergy_left = addEnergy_left;
        this.addEnergy_max = addEnergy_max;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SE_PLAYER_ENERGY_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_PLAYER_ENERGY_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteByte((byte) change);
        if (change == EnergyChangeFlag.Both)
        {
            writer.WriteSInt32(addEnergy_left);
            writer.WriteSInt32(addEnergy_max);
        }
        else if (change == EnergyChangeFlag.Left)
        {
            writer.WriteSInt32(addEnergy_left);
        }
        else if (change == EnergyChangeFlag.Max)
        {
            writer.WriteSInt32(addEnergy_max);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        change = (EnergyChangeFlag) reader.ReadByte();
        if (change == EnergyChangeFlag.Both)
        {
            addEnergy_left = reader.ReadSInt32();
            addEnergy_max = reader.ReadSInt32();
        }
        else if (change == EnergyChangeFlag.Left)
        {
            addEnergy_left = reader.ReadSInt32();
        }
        else if (change == EnergyChangeFlag.Max)
        {
            addEnergy_max = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [change]=" + change;
        log += " [addEnergy_left]=" + addEnergy_left;
        log += " [addEnergy_max]=" + addEnergy_max;
        return log;
    }

    public enum EnergyChangeFlag
    {
        Both = 0x00,
        Left = 0x01,
        Max = 0x02
    }
}