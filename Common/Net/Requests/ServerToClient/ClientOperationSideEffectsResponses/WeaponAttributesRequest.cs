using System.Collections;
using System.Collections.Generic;

public class WeaponAttributesRequest : ServerRequestBase
{
    public int clinetId;
    public int retinuePlaceIndex;
    public int weaponPlaceIndex;
    public WeaponAttributesChangeFlag change;
    public int addAttack;
    public int addEnergy;

    public WeaponAttributesRequest()
    {
    }

    public WeaponAttributesRequest(int clinetId, int retinuePlaceIndex, int weaponPlaceIndex, WeaponAttributesChangeFlag change, int addAttack = 0, int addEnergy = 0)
    {
        this.clinetId = clinetId;
        this.retinuePlaceIndex = retinuePlaceIndex;
        this.weaponPlaceIndex = weaponPlaceIndex;
        this.change = change;
        this.addAttack = addAttack;
        this.addEnergy = addEnergy;
    }

    public override int GetProtocol()
    {
        return NetProtocols.WEAPON_ATTRIBUTES_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "WEAPON_ATTRIBUTES_CHANGE";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(retinuePlaceIndex);
        writer.WriteSInt32(weaponPlaceIndex);
        writer.WriteByte((byte) change);
        if (change == WeaponAttributesChangeFlag.Both)
        {
            writer.WriteSInt32(addAttack);
            writer.WriteSInt32(addEnergy);
        }
        else if (change == WeaponAttributesChangeFlag.Attack)
        {
            writer.WriteSInt32(addAttack);
        }
        else if (change == WeaponAttributesChangeFlag.Energy)
        {
            writer.WriteSInt32(addEnergy);
        }
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        retinuePlaceIndex = reader.ReadSInt32();
        weaponPlaceIndex = reader.ReadSInt32();
        change = (WeaponAttributesChangeFlag) reader.ReadByte();
        if (change == WeaponAttributesChangeFlag.Both)
        {
            addAttack = reader.ReadSInt32();
            addEnergy = reader.ReadSInt32();
        }
        else if (change == WeaponAttributesChangeFlag.Attack)
        {
            addAttack = reader.ReadSInt32();
        }
        else if (change == WeaponAttributesChangeFlag.Energy)
        {
            addEnergy = reader.ReadSInt32();
        }
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId] " + clinetId;
        log += " [retinuePlaceIndex] " + retinuePlaceIndex;
        log += " [weaponPlaceIndex] " + weaponPlaceIndex;
        if (change == WeaponAttributesChangeFlag.Both)
        {
            log += " [addAttack] " + addAttack;
            log += " [addEnergy] " + addEnergy;
        }
        else if (change == WeaponAttributesChangeFlag.Attack)
        {
            log += " [addAttack] " + addAttack;
        }
        else if (change == WeaponAttributesChangeFlag.Energy)
        {
            log += " [addEnergy] " + addEnergy;
        }

        return log;
    }

    public enum WeaponAttributesChangeFlag
    {
        Both = 0x00,
        Attack = 0x01,
        Energy = 0x02
    }
}