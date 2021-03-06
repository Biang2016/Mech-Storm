﻿public class MechAttributesChangeRequest : ServerRequestBase
{
    public int clinetId;
    public int mechId;
    public int addLeftLife;
    public int addMaxLife;
    public int addAttack;
    public int addWeaponEnergy;
    public int addWeaponEnergyMax;
    public int addArmor;
    public int addShield;

    public MechAttributesChangeRequest()
    {
    }

    public MechAttributesChangeRequest(int clinetId, int mechId, int addLeftLife = 0, int addMaxLife = 0, int addAttack = 0, int addWeaponEnergy = 0, int addWeaponEnergyMax = 0, int addArmor = 0, int addShield = 0)
    {
        this.clinetId = clinetId;
        this.mechId = mechId;
        this.addLeftLife = addLeftLife;
        this.addMaxLife = addMaxLife;
        this.addAttack = addAttack;
        this.addWeaponEnergy = addWeaponEnergy;
        this.addWeaponEnergyMax = addWeaponEnergyMax;
        this.addArmor = addArmor;
        this.addShield = addShield;
    }

    public override NetProtocols GetProtocol()
    {
        return NetProtocols.SE_MECH_ATTRIBUTES_CHANGE;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(mechId);
        writer.WriteSInt32(addLeftLife);
        writer.WriteSInt32(addMaxLife);
        writer.WriteSInt32(addAttack);
        writer.WriteSInt32(addWeaponEnergy);
        writer.WriteSInt32(addWeaponEnergyMax);
        writer.WriteSInt32(addArmor);
        writer.WriteSInt32(addShield);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        clinetId = reader.ReadSInt32();
        mechId = reader.ReadSInt32();
        addLeftLife = reader.ReadSInt32();
        addMaxLife = reader.ReadSInt32();
        addAttack = reader.ReadSInt32();
        addWeaponEnergy = reader.ReadSInt32();
        addWeaponEnergyMax = reader.ReadSInt32();
        addArmor = reader.ReadSInt32();
        addShield = reader.ReadSInt32();
    }
}