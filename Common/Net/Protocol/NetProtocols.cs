using System.Collections;

public class NetProtocols
{
    #region  OutGame

    # region Server

    public const int HEART_BEAT_REQUEST = 0x00000001; //服务器发送的心跳包
    public const int CLIENT_ID_REQUEST = 0x00000002; //分配客户端Id
    public const int GAME_STOP_BY_LEAVE_REQUEST = 0x00000003; //由于玩家离开导致游戏结束

    #endregion

    # region Client

    public const int CARD_DECK_REQUEST = 0x00000101; //卡组信息
    public const int MATCH_REQUEST = 0x00000102; //申请匹配
    public const int CANCEL_MATCH_REQUEST = 0x00000103; //玩家取消匹配
    public const int LEAVE_GAME_REQUEST = 0x00000104; //玩家离开游戏

    #endregion

    #endregion

    #region InGame

    #region Server

    #region OperationResponse

    public const int GAME_START_RESPONSE = 0x00000200; //开始游戏_服务器响应

    public const int SUMMON_RETINUE_REQUEST_RESPONSE = 0x00000201; //召唤随从_服务器响应
    public const int EQUIP_WEAPON_REQUEST_RESPONSE = 0x00000202; //装备武器_服务器响应
    public const int EQUIP_SHIELD_REQUEST_RESPONSE = 0x00000203; //装备防具_服务器响应

    public const int RETINUE_ATTACK_RETINUE_REQUEST_RESPONSE = 0x00000204; //随从攻击随从_服务器响应

    public const int END_ROUND_REQUEST_RESPONSE = 0x00000205; //结束回合_服务器响应
    #endregion

    #region SideEffects

    public const int SE_SET_PLAYER = 0x00000207; //英雄信息

    public const int SE_PLAYER_TURN = 0x00000208; //切换玩家
    public const int SE_PLAYER_COST_CHANGE = 0x00000209; //英雄费用信息

    public const int SE_RETINUE_ATTRIBUTES_CHANGE = 0x00000210; //随从数值变化

    public const int SE_RETINUE_DIE = 0x00000213; //随从死亡

    public const int SE_BATTLEGROUND_ADD_RETINUE = 0x00000214; //战场增加随从
    public const int SE_BATTLEGROUND_REMOVE_RETINUE = 0x00000215; //战场减少随从

    public const int SE_CARDDECT_LEFT_CHANGE = 0x00000216; //发送剩余牌数
    public const int SE_DRAW_CARD = 0x00000217; //抽一张牌
    public const int SE_DROP_CARD = 0x00000218; //手牌弃牌
    public const int SE_USE_CARD = 0x00000219; //手牌用牌

    public const int SE_EQUIP_WEAPON_SERVER_REQUEST = 0x00000220; //装备武器
    public const int SE_EQUIP_SHIELD_SERVER_REQUEST = 0x00000221; //装备防具

    public const int SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST = 0x00000222; //随从攻击随从
    public const int SE_RETINUE_EFFECT = 0x00000223; //随从特效

    #endregion



    #endregion

    #region ClientOperation

    public const int SUMMON_RETINUE_REQUEST = 0x00000301; //召唤随从
    public const int EQUIP_WEAPON_REQUEST = 0x00000302; //装备武器
    public const int EQUIP_SHIELD_REQUEST = 0x00000303; //装备防具

    public const int RETINUE_ATTACK_RETINUE_REQUEST = 0x00000304; //随从攻击随从

    public const int END_ROUND_REQUEST = 0x00000305; //结束回合

    #endregion

    #endregion
}