using System.Collections.Generic;

public class ExecutorInfo
{
    public const int EXECUTE_INFO_NONE = -99999;
    public const int EXECUTOR_ID_EVENTS = -1;

    public int SideEffectExecutorID;
    public int ClientId;
    public int MechId;
    public int CardId;
    public int CardInstanceId;
    public int EquipId;

    public List<int> TargetClientIds = new List<int>();
    public List<int> TargetMechIds = new List<int>();
    public List<int> TargetCardInstanceIds = new List<int>();
    public List<int> TargetEquipIds = new List<int>();
    public SortedDictionary<ExecutorInfoValues, int> ValueDictionary = new SortedDictionary<ExecutorInfoValues, int>();
    public bool IsPlayerBuff;

    public ExecutorInfo(
        int clientId,
        int sideEffectExecutorID = EXECUTOR_ID_EVENTS,
        int mechId = EXECUTE_INFO_NONE,
        int cardId = EXECUTE_INFO_NONE,
        int cardInstanceId = EXECUTE_INFO_NONE,
        int equipId = EXECUTE_INFO_NONE,
        List<int> targetClientIds = null,
        List<int> targetMechIds = null,
        List<int> targetCardInstanceIds = null,
        List<int> targetEquipIds = null,
        SortedDictionary<ExecutorInfoValues, int> valueDictionary = null,
        bool isPlayerBuff = false)
    {
        ClientId = clientId;
        SideEffectExecutorID = sideEffectExecutorID;
        MechId = mechId;
        CardId = cardId;
        CardInstanceId = cardInstanceId;
        EquipId = equipId;
        TargetClientIds = targetClientIds ?? new List<int>();
        TargetMechIds = targetMechIds ?? new List<int>();
        TargetCardInstanceIds = targetCardInstanceIds ?? new List<int>();
        TargetEquipIds = targetEquipIds ?? new List<int>();
        ValueDictionary = valueDictionary ?? new SortedDictionary<ExecutorInfoValues, int>();
        IsPlayerBuff = isPlayerBuff;
    }

    public enum ExecutorInfoValues
    {
        Metal_Use,
        Energy_Add,
        Energy_Use,
        ShipLife_Add,
        ShipLife_Injury,
        MechLife_Add,
        MechLife_Injury,
        Damage,
        DamageToHero,
        DamageToSoldier,
        DamageToShip,
        Heal,
        Armor_Add,
        Armor_Reduce,
        Shield_Add,
        Shield_Reduce,
        Mech_AffectedCount,
        CardCount,
    }

    public ExecutorInfo Clone()
    {
        return new ExecutorInfo(
            ClientId,
            SideEffectExecutorID,
            MechId,
            CardId,
            CardInstanceId,
            EquipId,
            CloneVariantUtils.List(TargetClientIds),
            CloneVariantUtils.List(TargetMechIds),
            CloneVariantUtils.List(TargetCardInstanceIds),
            CloneVariantUtils.List(TargetEquipIds),
            CloneVariantUtils.SortedDictionary(ValueDictionary),
            IsPlayerBuff);
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ClientId);
        writer.WriteSInt32(SideEffectExecutorID);
        writer.WriteSInt32(MechId);

        writer.WriteSInt32(CardId);
        writer.WriteSInt32(CardInstanceId);

        writer.WriteSInt32(EquipId);

        writer.WriteSInt32(TargetClientIds.Count);
        foreach (int targetClientId in TargetClientIds)
        {
            writer.WriteSInt32(targetClientId);
        }

        writer.WriteSInt32(TargetMechIds.Count);
        foreach (int targetMechId in TargetMechIds)
        {
            writer.WriteSInt32(targetMechId);
        }

        writer.WriteSInt32(TargetCardInstanceIds.Count);
        foreach (int targetCardInstanceId in TargetCardInstanceIds)
        {
            writer.WriteSInt32(targetCardInstanceId);
        }

        writer.WriteSInt32(TargetEquipIds.Count);
        foreach (int targetEquipId in TargetEquipIds)
        {
            writer.WriteSInt32(targetEquipId);
        }

        writer.WriteSInt32(ValueDictionary.Count);
        foreach (KeyValuePair<ExecutorInfoValues, int> kv in ValueDictionary)
        {
            writer.WriteSInt32((int) kv.Key);
            writer.WriteSInt32(kv.Value);
        }

        writer.WriteByte((byte) (IsPlayerBuff ? 0x01 : 0x00));
    }

    public static ExecutorInfo Deserialize(DataStream reader)
    {
        int ClientId = reader.ReadSInt32();
        int SideEffectExecutorID = reader.ReadSInt32();
        int MechId = reader.ReadSInt32();

        int CardId = reader.ReadSInt32();
        int CardInstanceId = reader.ReadSInt32();

        int EquipId = reader.ReadSInt32();

        int count = reader.ReadSInt32();
        List<int> TargetClientIds = new List<int>();
        for (int i = 0; i < count; i++)
        {
            TargetClientIds.Add(reader.ReadSInt32());
        }

        count = reader.ReadSInt32();
        List<int> TargetMechIds = new List<int>();
        for (int i = 0; i < count; i++)
        {
            TargetMechIds.Add(reader.ReadSInt32());
        }

        count = reader.ReadSInt32();
        List<int> TargetCardInstanceIds = new List<int>();
        for (int i = 0; i < count; i++)
        {
            TargetCardInstanceIds.Add(reader.ReadSInt32());
        }

        count = reader.ReadSInt32();
        List<int> TargetEquipIds = new List<int>();
        for (int i = 0; i < count; i++)
        {
            TargetEquipIds.Add(reader.ReadSInt32());
        }

        count = reader.ReadSInt32();
        SortedDictionary<ExecutorInfoValues, int> valueDictionary = new SortedDictionary<ExecutorInfoValues, int>();
        for (int i = 0; i < count; i++)
        {
            ExecutorInfoValues key = (ExecutorInfoValues) reader.ReadSInt32();
            int value = reader.ReadSInt32();
            valueDictionary.Add(key, value);
        }

        bool IsPlayerBuff = reader.ReadByte() == 0x01;

        return new ExecutorInfo(
            ClientId,
            SideEffectExecutorID,
            MechId,
            CardId,
            CardInstanceId,
            EquipId,
            TargetClientIds,
            TargetMechIds,
            TargetCardInstanceIds,
            TargetEquipIds,
            valueDictionary,
            IsPlayerBuff
        );
    }
}