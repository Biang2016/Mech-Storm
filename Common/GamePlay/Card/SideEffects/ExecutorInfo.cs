    public class ExecutorInfo
    {
        public const int EXECUTE_INFO_NONE = -999;
        public const int EXECUTOR_ID_EVENTS = -1;

        public int SideEffectExecutorID;
        public int ClientId;
        public int TargetClientId;
        public int RetinueId;
        public int TargetRetinueId;
        public int CardId;
        public int CardInstanceId;
        public int EquipId;
        public int TargetEquipId;
        public int Value;
        public bool IsPlayerBuff;

        public ExecutorInfo(int clientId, int targetClientId = EXECUTE_INFO_NONE, int sideEffectExecutorID = EXECUTOR_ID_EVENTS, int retinueId = EXECUTE_INFO_NONE, int targetRetinueId = EXECUTE_INFO_NONE, int cardId = EXECUTE_INFO_NONE, int cardInstanceId = EXECUTE_INFO_NONE,
            int equipId = EXECUTE_INFO_NONE, int targetEquipId = EXECUTE_INFO_NONE, int value = 0, bool isPlayerBuff = false)
        {
            ClientId = clientId;
            TargetClientId = targetClientId;
            SideEffectExecutorID = sideEffectExecutorID;
            RetinueId = retinueId;
            TargetRetinueId = targetRetinueId;
            CardId = cardId;
            CardInstanceId = cardInstanceId;
            EquipId = equipId;
            TargetEquipId = targetEquipId;
            Value = value;
            IsPlayerBuff = isPlayerBuff;
        }

        public ExecutorInfo Clone()
        {
            return new ExecutorInfo(ClientId, TargetClientId, SideEffectExecutorID, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId, Value);
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32(ClientId);
            writer.WriteSInt32(TargetClientId);
            writer.WriteSInt32(SideEffectExecutorID);
            writer.WriteSInt32(RetinueId);
            writer.WriteSInt32(TargetRetinueId);
            writer.WriteSInt32(CardId);
            writer.WriteSInt32(CardInstanceId);
            writer.WriteSInt32(EquipId);
            writer.WriteSInt32(TargetEquipId);
            writer.WriteSInt32(Value);
        }

        public static ExecutorInfo Deserialize(DataStream reader)
        {
            int ClientId = reader.ReadSInt32();
            int TargetClientId = reader.ReadSInt32();
            int SideEffectExecutorID = reader.ReadSInt32();
            int RetinueId = reader.ReadSInt32();
            int TargetRetinueId = reader.ReadSInt32();
            int CardId = reader.ReadSInt32();
            int CardInstanceId = reader.ReadSInt32();
            int EquipId = reader.ReadSInt32();
            int TargetEquipId = reader.ReadSInt32();
            int Value = reader.ReadSInt32();
            return new ExecutorInfo(ClientId, TargetClientId, SideEffectExecutorID, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId, Value);
        }
    }
