﻿using System.Collections;
using System.Collections.Generic;

public class EquipWeaponRequest : ClientRequestBase
{
    public int handCardIndex;
    public int battleGroundIndex;
    public int weaponPlaceIndex;

    public EquipWeaponRequest()
    {
    }

    public EquipWeaponRequest(int clientId,  int handCardIndex, int battleGroundIndex, int weaponPlaceIndex):base(clientId)
    {
        this.handCardIndex = handCardIndex;
        this.battleGroundIndex = battleGroundIndex;
        this.weaponPlaceIndex = weaponPlaceIndex;
    }

    public override int GetProtocol()
    {
        return NetProtocols.EQUIP_WEAPON_REQUEST;
    }

    public override string GetProtocolName()
    {
        return "EQUIP_WEAPON_REQUEST";
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(handCardIndex);
        writer.WriteSInt32(battleGroundIndex);
        writer.WriteSInt32(weaponPlaceIndex);
    }

    public override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        handCardIndex = reader.ReadSInt32();
        battleGroundIndex = reader.ReadSInt32();
        weaponPlaceIndex = reader.ReadSInt32();
    }

    public override string DeserializeLog()
    {
        string log = "";
        log += " [handCardIndex]=" + handCardIndex;
        log += " [battleGroundIndex]=" + battleGroundIndex;
        log += " [weaponPlaceIndex]=" + weaponPlaceIndex;
        return log;
    }
}