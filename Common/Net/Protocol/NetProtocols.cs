using System.Collections;

public class NetProtocols
{
    #region  OutGame

    # region Server

    public const int HEART_BEAT = 0x00000001; //服务器发送的心跳包
    public const int SEND_CLIENT_ID = 0x00000002; //分配客户端Id
    public const int GAME_STOP_BY_LEAVE = 0x00000003; //由于玩家离开导致游戏结束

    #endregion

    # region Client

    public const int CARD_DECK_INFO = 0x00000101; //卡组信息
    public const int Match = 0x00000102; //申请匹配
    public const int CANCEL_MATCH = 0x00000103; //玩家取消匹配
    public const int LEAVE_GAME = 0x00000104; //玩家离开游戏

    #endregion

    #endregion

    # region InGame

    #region Server

    public const int PLAYER = 0x00000201; //英雄信息
    public const int PLAYER_TURN = 0x00000202; //切换玩家
    public const int DRAW_CARD = 0x00000203; //抽一张牌
    public const int PLAYER_COST_CHANGE = 0x00000204; //英雄费用信息

    public const int SUMMON_RETINUE_RESPONSE = 0x00000205; //召唤随从_服务器响应
    public const int EQUIP_WEAPON_RESPONSE = 0x00000206; //装备武器_服务器响应
    public const int EQUIP_SHIELD_RESPONSE = 0x00000207; //装备防具_服务器响应

    public const int RETINUE_ATTACK_RETINUE_RESPONSE = 0x00000208; //随从攻击随从_服务器响应

    public const int RETINUE_ATTRIBUTES_CHANGE = 0x00000209; //随从数值变化
    public const int WEAPON_ATTRIBUTES_CHANGE = 0x00000210; //武器模块数值变化
    public const int SHIELD_ATTRIBUTES_CHANGE = 0x00000211; //防具模块数值变化

    #endregion

    #region Client

    public const int SUMMON_RETINUE = 0x00000301; //召唤随从
    public const int EQUIP_WEAPON = 0x00000302; //装备武器
    public const int EQUIP_SHIELD = 0x00000303; //装备防具

    public const int RETINUE_ATTACK_RETINUE = 0x00000304; //随从攻击随从

    public const int END_ROUND = 0x00000305; //结束回合

    #endregion

    #endregion
}