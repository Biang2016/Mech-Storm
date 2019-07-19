using System;

public class BattleStatistics
{
    public int Level;

    public int TotalDamage;
    public int DamageToMech;
    public int DamageToShip;

    public int TotalInjury;
    public int MechInjury;
    public int ShipInjury;

    public int TotalHeal;
    public int ShipHeal;
    public int MechHeal;

    public int TotalSummon;
    public int SoldierSummon;
    public int HeroSummon;

    public int FinalHealth;
    public float FinalHealthRatio;

    public int TotalKill;
    public int SoldierKill;
    public int HeroKill;

    public int TotalLost;
    public int SoldierLost;
    public int HeroLost;

    public int Draws;
    public int Rounds;

    public int TotalGetEnergy;
    public int TotalUseEnergy;
    public int TotalUseMetal;

    public int UseCards;
    public int UseCards_Mech;
    public int UseCards_Soldier;
    public int UseCards_Hero;
    public int UseCards_Equipment;
    public int UseCards_Equipment_Weapon;
    public int UseCards_Equipment_Weapon_Sword;
    public int UseCards_Equipment_Weapon_Gun;
    public int UseCards_Equipment_Weapon_SniperGun;
    public int UseCards_Equipment_Shield;
    public int UseCards_Equipment_Shield_Armor;
    public int UseCards_Equipment_Shield_Shield;
    public int UseCards_Equipment_Shield_Mixed;
    public int UseCards_Equipment_Pack;
    public int UseCards_Equipment_MA;
    public int UseCards_Spell;
    public int UseCards_Energy;

    #region Calculation

    public const int LEVEL_CRYSTAL = 10;
    public const float DAMAGE_CRYSTAL = 0.1f;
    public const int DAMAGE_CRYSTAL_MAX = 10;
    public const float KILL_CRYSTAL = 0.5f;
    public const int KILL_CRYSTAL_MAX = 10;
    public const int HEALTH_CRYSTAL = 1;

    public int levelCrystal => Level * LEVEL_CRYSTAL;
    public int healthCrystal => ((int) Math.Round(FinalHealthRatio * 100)) / 20 * HEALTH_CRYSTAL;

    public int damageCrystal
    {
        get
        {
            int temp = (int) (TotalDamage * DAMAGE_CRYSTAL);
            if (temp >= DAMAGE_CRYSTAL_MAX)
            {
                return DAMAGE_CRYSTAL_MAX;
            }
            else
            {
                return temp;
            }
        }
    }

    public int killCrystal
    {
        get
        {
            int temp = (int) (TotalKill * KILL_CRYSTAL);
            if (temp >= KILL_CRYSTAL_MAX)
            {
                return KILL_CRYSTAL_MAX;
            }
            else
            {
                return temp;
            }
        }
    }

    public int totalCrystal => levelCrystal + healthCrystal + killCrystal + damageCrystal;

    #endregion

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Level);

        writer.WriteSInt32(TotalDamage);
        writer.WriteSInt32(DamageToMech);
        writer.WriteSInt32(DamageToShip);

        writer.WriteSInt32(TotalInjury);
        writer.WriteSInt32(MechInjury);
        writer.WriteSInt32(ShipInjury);

        writer.WriteSInt32(TotalHeal);
        writer.WriteSInt32(MechHeal);
        writer.WriteSInt32(ShipHeal);

        writer.WriteSInt32(TotalSummon);
        writer.WriteSInt32(SoldierSummon);
        writer.WriteSInt32(HeroSummon);

        writer.WriteSInt32(FinalHealth);
        writer.WriteSInt32((int) (FinalHealthRatio * 1000));

        writer.WriteSInt32(TotalKill);
        writer.WriteSInt32(SoldierKill);
        writer.WriteSInt32(HeroKill);

        writer.WriteSInt32(TotalLost);
        writer.WriteSInt32(SoldierLost);
        writer.WriteSInt32(HeroLost);

        writer.WriteSInt32(Draws);
        writer.WriteSInt32(Rounds);

        writer.WriteSInt32(TotalGetEnergy);
        writer.WriteSInt32(TotalUseEnergy);
        writer.WriteSInt32(TotalUseMetal);

        writer.WriteSInt32(UseCards);
        writer.WriteSInt32(UseCards_Mech);
        writer.WriteSInt32(UseCards_Soldier);
        writer.WriteSInt32(UseCards_Hero);
        writer.WriteSInt32(UseCards_Equipment);
        writer.WriteSInt32(UseCards_Equipment_Weapon);
        writer.WriteSInt32(UseCards_Equipment_Weapon_Sword);
        writer.WriteSInt32(UseCards_Equipment_Weapon_Gun);
        writer.WriteSInt32(UseCards_Equipment_Weapon_SniperGun);
        writer.WriteSInt32(UseCards_Equipment_Shield);
        writer.WriteSInt32(UseCards_Equipment_Shield_Armor);
        writer.WriteSInt32(UseCards_Equipment_Shield_Shield);
        writer.WriteSInt32(UseCards_Equipment_Shield_Mixed);
        writer.WriteSInt32(UseCards_Equipment_Pack);
        writer.WriteSInt32(UseCards_Equipment_MA);
        writer.WriteSInt32(UseCards_Spell);
        writer.WriteSInt32(UseCards_Energy);
    }

    public static BattleStatistics Deserialize(DataStream reader)
    {
        BattleStatistics bsm = new BattleStatistics();
        bsm.Level = reader.ReadSInt32();

        bsm.TotalDamage = reader.ReadSInt32();
        bsm.DamageToMech = reader.ReadSInt32();
        bsm.DamageToShip = reader.ReadSInt32();

        bsm.TotalInjury = reader.ReadSInt32();
        bsm.MechInjury = reader.ReadSInt32();
        bsm.ShipInjury = reader.ReadSInt32();

        bsm.TotalHeal = reader.ReadSInt32();
        bsm.MechHeal = reader.ReadSInt32();
        bsm.ShipHeal = reader.ReadSInt32();

        bsm.TotalSummon = reader.ReadSInt32();
        bsm.SoldierSummon = reader.ReadSInt32();
        bsm.HeroSummon = reader.ReadSInt32();

        bsm.FinalHealth = reader.ReadSInt32();
        bsm.FinalHealthRatio = (float) reader.ReadSInt32() / 1000;

        bsm.TotalKill = reader.ReadSInt32();
        bsm.SoldierKill = reader.ReadSInt32();
        bsm.HeroKill = reader.ReadSInt32();

        bsm.TotalLost = reader.ReadSInt32();
        bsm.SoldierLost = reader.ReadSInt32();
        bsm.HeroLost = reader.ReadSInt32();

        bsm.Draws = reader.ReadSInt32();
        bsm.Rounds = reader.ReadSInt32();

        bsm.TotalGetEnergy = reader.ReadSInt32();
        bsm.TotalUseEnergy = reader.ReadSInt32();
        bsm.TotalUseMetal = reader.ReadSInt32();

        bsm.UseCards = reader.ReadSInt32();
        bsm.UseCards_Mech = reader.ReadSInt32();
        bsm.UseCards_Soldier = reader.ReadSInt32();
        bsm.UseCards_Hero = reader.ReadSInt32();
        bsm.UseCards_Equipment = reader.ReadSInt32();
        bsm.UseCards_Equipment_Weapon = reader.ReadSInt32();
        bsm.UseCards_Equipment_Weapon_Sword = reader.ReadSInt32();
        bsm.UseCards_Equipment_Weapon_Gun = reader.ReadSInt32();
        bsm.UseCards_Equipment_Weapon_SniperGun = reader.ReadSInt32();
        bsm.UseCards_Equipment_Shield = reader.ReadSInt32();
        bsm.UseCards_Equipment_Shield_Armor = reader.ReadSInt32();
        bsm.UseCards_Equipment_Shield_Shield = reader.ReadSInt32();
        bsm.UseCards_Equipment_Shield_Mixed = reader.ReadSInt32();
        bsm.UseCards_Equipment_Pack = reader.ReadSInt32();
        bsm.UseCards_Equipment_MA = reader.ReadSInt32();
        bsm.UseCards_Spell = reader.ReadSInt32();
        bsm.UseCards_Energy = reader.ReadSInt32();
        return bsm;
    }

    public void UseCard(CardInfo_Base cardInfo)
    {
        UseCards++;
        switch (cardInfo.BaseInfo.CardType)
        {
            case CardTypes.Mech:
            {
                UseCards_Mech++;
                if (cardInfo.MechInfo.IsSoldier)
                {
                    UseCards_Soldier++;
                }
                else
                {
                    UseCards_Hero++;
                }

                break;
            }
            case CardTypes.Equip:
            {
                UseCards_Equipment++;
                switch (cardInfo.EquipInfo.SlotType)
                {
                    case SlotTypes.Weapon:
                    {
                        UseCards_Equipment_Weapon++;
                        switch (cardInfo.WeaponInfo.WeaponType)
                        {
                            case WeaponTypes.Sword:
                            {
                                UseCards_Equipment_Weapon_Sword++;
                                break;
                            }
                            case WeaponTypes.Gun:
                            {
                                UseCards_Equipment_Weapon_Gun++;
                                break;
                            }
                            case WeaponTypes.SniperGun:
                            {
                                UseCards_Equipment_Weapon_SniperGun++;
                                break;
                            }
                        }

                        break;
                    }
                    case SlotTypes.Shield:
                    {
                        UseCards_Equipment_Shield++;
                        switch (cardInfo.ShieldInfo.ShieldType)
                        {
                            case ShieldTypes.Armor:
                            {
                                UseCards_Equipment_Shield_Armor++;
                                break;
                            }
                            case ShieldTypes.Shield:
                            {
                                UseCards_Equipment_Shield_Shield++;
                                break;
                            }
                            case ShieldTypes.Mixed:
                            {
                                UseCards_Equipment_Shield_Mixed++;
                                break;
                            }
                        }

                        break;
                    }
                    case SlotTypes.Pack:
                    {
                        //TODO pack types
                        UseCards_Equipment_Pack++;
                        break;
                    }
                    case SlotTypes.MA:
                    {
                        //TODO ma types
                        UseCards_Equipment_MA++;
                        break;
                    }
                }

                break;
            }
            case CardTypes.Spell:
            {
                UseCards_Spell++;
                break;
            }
            case CardTypes.Energy:
            {
                UseCards_Energy++;
                break;
            }
        }
    }
}