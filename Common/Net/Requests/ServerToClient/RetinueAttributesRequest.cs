using System.Collections;
using System.Collections.Generic;

public class RetinueAttributesRequest : ServerRequestBase
{
    public int clinetId;
    public int retinuePlaceIndex;
    public RetinueAttributesChangeFlag change; 
    public int addLeftLife;
    public int addMaxLife;
    public int addAttack;
    public int addArmor;
    public int addShield;

    public RetinueAttributesRequest()
    {
    }

    public RetinueAttributesRequest(int clinetId, int retinuePlaceIndex, RetinueAttributesChangeFlag change, int addLeftLife = 0, int addMaxLife = 0, int addAttack = 0, int addArmor = 0, int addShield = 0)
    {
        this.clinetId = clinetId;
        this.retinuePlaceIndex = retinuePlaceIndex;
        this.change = change;
        this.addLeftLife = addLeftLife;
        this.addMaxLife = addMaxLife;
        this.addAttack = addAttack;
        this.addArmor = addArmor;
        this.addShield = addShield;
    }

    public override int GetProtocol()
    {
        return NetProtocols.RETINUE_ATTRIBUTES_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "RETINUE_ATTRIBUTES_CHANGE";
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(retinuePlaceIndex);
        writer.WriteByte((byte) change);
        if (change == RetinueAttributesChangeFlag.ALL)
        {
            writer.WriteSInt32(addLeftLife);
            writer.WriteSInt32(addMaxLife);
            writer.WriteSInt32(addAttack);
            writer.WriteSInt32(addArmor);
            writer.WriteSInt32(addShield);
        }
        else if (change == RetinueAttributesChangeFlag.LeftLife)
        {
            writer.WriteSInt32(addLeftLife);
        }
        else if (change == RetinueAttributesChangeFlag.MaxLife)
        {
            writer.WriteSInt32(addMaxLife);
        }
        else if (change == RetinueAttributesChangeFlag.Attack)
        {
            writer.WriteSInt32(addAttack);
        }
        else if (change == RetinueAttributesChangeFlag.Armor)
        {
            writer.WriteSInt32(addArmor);
        }
        else if (change == RetinueAttributesChangeFlag.Shield)
        {
            writer.WriteSInt32(addShield);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        retinuePlaceIndex = reader.ReadSInt32();
        change = (RetinueAttributesChangeFlag) reader.ReadByte();
        if (change == RetinueAttributesChangeFlag.ALL)
        {
            addLeftLife = reader.ReadSInt32();
            addMaxLife = reader.ReadSInt32();
            addAttack = reader.ReadSInt32();
            addArmor = reader.ReadSInt32();
            addShield = reader.ReadSInt32();
        }
        else if (change == RetinueAttributesChangeFlag.LeftLife)
        {
            addLeftLife = reader.ReadSInt32();
        }
        else if (change == RetinueAttributesChangeFlag.MaxLife)
        {
            addMaxLife = reader.ReadSInt32();
        }
        else if (change == RetinueAttributesChangeFlag.Attack)
        {
            addAttack = reader.ReadSInt32();
        }
        else if (change == RetinueAttributesChangeFlag.Armor)
        {
            addArmor = reader.ReadSInt32();
        }
        else if (change == RetinueAttributesChangeFlag.Shield)
        {
            addShield = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId] " + clinetId;
        log += " [retinuePlaceIndex] " + retinuePlaceIndex;
        if (change == RetinueAttributesChangeFlag.ALL)
        {
            log += " [addLeftLife] " + addLeftLife;
            log += " [addMaxLife] " + addMaxLife;
            log += " [addAttack] " + addAttack;
            log += " [addArmor] " + addArmor;
            log += " [addShield] " + addShield;
        }
        else if (change == RetinueAttributesChangeFlag.LeftLife)
        {
            log += " [addLeftLife] " + addLeftLife;
        }
        else if (change == RetinueAttributesChangeFlag.MaxLife)
        {
            log += " [addMaxLife] " + addMaxLife;
        }
        else if (change == RetinueAttributesChangeFlag.Attack)
        {
            log += " [addAttack] " + addAttack;
        }
        else if (change == RetinueAttributesChangeFlag.Armor)
        {
            log += " [addArmor] " + addArmor;
        }
        else if (change == RetinueAttributesChangeFlag.Shield)
        {
            log += " [addShield] " + addShield;
        }

        return log;
    }

    public enum RetinueAttributesChangeFlag
    {
        ALL = 0x00,
        LeftLife = 0x01,
        MaxLife = 0x02,
        Attack = 0x03,
        Armor = 0x04,
        Shield = 0x05,
    }
}