public class Const
{
    public const int TARGET_MECH_SELECT_NONE = -2; //无指定目标的随从
    public const int CARD_INSTANCE_ID_NONE = -1; //无对应卡牌实例（凭空生成的牌，不参与卡组循环）

    public enum SpecialMechID
    {
        Empty = -1, // 召唤Mech预览时的ID
        ClientTempMechIDNormal = -2, // 临时MechID的默认值
    }
}