public class RetinueAttributesChangeRequest : ServerRequestBase
{
    public int clinetId;
    public int retinueId;
    public int addLeftLife;
    public int addMaxLife;
    public int addAttack;
    public int addWeaponEnergy;
    public int addWeaponEnergyMax;
    public int addArmor;
    public int addShield;

    public RetinueAttributesChangeRequest()
    {
    }

    public RetinueAttributesChangeRequest(int clinetId, int retinueId, int addLeftLife = 0, int addMaxLife = 0, int addAttack = 0, int addWeaponEnergy = 0, int addWeaponEnergyMax = 0, int addArmor = 0, int addShield = 0)
    {
        this.clinetId = clinetId;
        this.retinueId = retinueId;
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
        return NetProtocols.SE_RETINUE_ATTRIBUTES_CHANGE;
    }

    public override string GetProtocolName()
    {
        return "SE_RETINUE_ATTRIBUTES_CHANGE";
    }


    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(clinetId);
        writer.WriteSInt32(retinueId);
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
        retinueId = reader.ReadSInt32();
        addLeftLife = reader.ReadSInt32();
        addMaxLife = reader.ReadSInt32();
        addAttack = reader.ReadSInt32();
        addWeaponEnergy = reader.ReadSInt32();
        addWeaponEnergyMax = reader.ReadSInt32();
        addArmor = reader.ReadSInt32();
        addShield = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = base.DeserializeLog();
        log += " [clinetId]=" + clinetId;
        log += " [retinueId]=" + retinueId;
        log += " [addLeftLife]=" + addLeftLife;
        log += " [addMaxLife]=" + addMaxLife;
        log += " [addAttack]=" + addAttack;
        log += " [addWeaponEnergy]=" + addWeaponEnergy;
        log += " [addWeaponEnergyMax]=" + addWeaponEnergyMax;
        log += " [addArmor]=" + addArmor;
        log += " [addShield]=" + addShield;
        return log;
    }
}