using System.Collections;
using System.Collections.Generic;

public class ShieldAttributesRequest : ServerRequestBase
{
    public int clinetId;
    public int retinuePlaceIndex;
    public int shieldPlaceIndex;
    public ShieldAttributesChangeFlag change;
    public int addArmor;
    public int addArmorMax;
    public int addShield;
    public int addShieldMax;

    public ShieldAttributesRequest()
    {
    }

    public ShieldAttributesRequest(int clinetId, int retinuePlaceIndex, int shieldPlaceIndex, ShieldAttributesChangeFlag change, int addArmor = 0, int addArmorMax = 0, int addShield = 0, int addShieldMax = 0)
    {
        this.clinetId = clinetId;
        this.retinuePlaceIndex = retinuePlaceIndex;
        this.shieldPlaceIndex = shieldPlaceIndex;
        this.change = change;
        this.addArmor = addArmor;
        this.addArmorMax = addArmorMax;
        this.addShield = addShield;
        this.addShieldMax = addShieldMax;
    }

    public override int GetProtocol()
    {
        return NetProtocols.SHIELD_ATTRIBUTES_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SHIELD_ATTRIBUTES_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(retinuePlaceIndex);
        writer.WriteSInt32(shieldPlaceIndex);
        writer.WriteByte((byte) change);
        if (change == ShieldAttributesChangeFlag.All)
        {
            writer.WriteSInt32(addArmor);
            writer.WriteSInt32(addArmorMax);
            writer.WriteSInt32(addShield);
            writer.WriteSInt32(addShieldMax);
        }
        else if (change == ShieldAttributesChangeFlag.Armor)
        {
            writer.WriteSInt32(addArmor);
        }
        else if (change == ShieldAttributesChangeFlag.ArmorMax)
        {
            writer.WriteSInt32(addArmorMax);
        }
        else if (change == ShieldAttributesChangeFlag.Shield)
        {
            writer.WriteSInt32(addShield);
        }
        else if (change == ShieldAttributesChangeFlag.ShieldMax)
        {
            writer.WriteSInt32(addShieldMax);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        retinuePlaceIndex = reader.ReadSInt32();
        shieldPlaceIndex = reader.ReadSInt32();
        change = (ShieldAttributesChangeFlag) reader.ReadByte();
        if (change == ShieldAttributesChangeFlag.All)
        {
            addArmor = reader.ReadSInt32();
            addArmorMax = reader.ReadSInt32();
            addShield = reader.ReadSInt32();
            addShieldMax = reader.ReadSInt32();
        }
        else if (change == ShieldAttributesChangeFlag.Armor)
        {
            addArmor = reader.ReadSInt32();
        }
        else if (change == ShieldAttributesChangeFlag.ArmorMax)
        {
            addArmorMax = reader.ReadSInt32();
        }
        else if (change == ShieldAttributesChangeFlag.Shield)
        {
            addShield = reader.ReadSInt32();
        }
        else if (change == ShieldAttributesChangeFlag.ShieldMax)
        {
            addShieldMax = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId] " + clinetId;
        log += " [retinuePlaceIndex] " + retinuePlaceIndex;
        log += " [shieldPlaceIndex] " + shieldPlaceIndex;

        if (change == ShieldAttributesChangeFlag.All)
        {
            log += " [addArmor] " + addArmor;
            log += " [addArmorMax] " + addArmorMax;
            log += " [addShield] " + addShield;
            log += " [addShieldMax] " + addShieldMax;
        }
        else if (change == ShieldAttributesChangeFlag.Armor)
        {
            log += " [addArmor] " + addArmor;
        }
        else if (change == ShieldAttributesChangeFlag.ArmorMax)
        {
            log += " [addArmorMax] " + addArmorMax;
        }
        else if (change == ShieldAttributesChangeFlag.Shield)
        {
            log += " [addShield] " + addShield;
        }
        else if (change == ShieldAttributesChangeFlag.ShieldMax)
        {
            log += " [addShieldMax] " + addShieldMax;
        }

        return log;
    }

    public enum ShieldAttributesChangeFlag
    {
        All = 0x00,
        Armor = 0x01,
        ArmorMax = 0x02,
        Shield = 0x03,
        ShieldMax = 0x04,
    }
}