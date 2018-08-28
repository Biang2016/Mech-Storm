public class NetProtocols
{
    #region  OutGame

    # region Server

    public const int CLIENT_ID_REQUEST = 0x00000000; //服务器发送的客户端ID号
    public const int HEART_BEAT_REQUEST = 0x00000001; //服务器发送的心跳包
    public const int CLIENT_BUILDINFOS_REQUEST = 0x00000003; //玩家所有卡组信息
    public const int GAME_STOP_BY_LEAVE_REQUEST = 0x00000004; //由于玩家离开导致游戏结束
    public const int RANDOM_NUMBER_SEED_REQUEST = 0x00000005; //分配客户端随机数种子
    public const int REGISTER_RESULT_REQUEST = 0x00000006; //注册结果
    public const int LOGIN_RESULT_REQUEST = 0x00000007; //登录结果
    public const int LOGOUT_RESULT_REQUEST = 0x00000008; //登出结果
    public const int CREATE_BUILD_REQUEST_RESPONSE = 0x00000009; //新卡组回应
    public const int BUILD_UPDATE_RESPONSE = 0x00000010; //更新卡组
    public const int DELETE_BUILD_REQUEST_RESPONSE = 0x00000011; //删除卡组

    #endregion

    # region Client

    public const int CARD_DECK_REQUEST = 0x00000101; //卡组信息
    public const int MATCH_REQUEST = 0x00000102; //申请匹配
    public const int CANCEL_MATCH_REQUEST = 0x00000103; //玩家取消匹配
    public const int LEAVE_GAME_REQUEST = 0x00000104; //玩家离开游戏
    public const int REGISTER_REQUEST = 0x00000105; //注册
    public const int LOGIN_REQUEST = 0x00000106; //登录
    public const int LOGOUT_REQUEST = 0x00000107; //登出
    public const int BUILD_REQUEST = 0x00000108; //编辑卡组
    public const int DELETE_BUILD_REQUEST = 0x00000109; //删除卡组

    #endregion

    #endregion

    #region InGame

    #region Server

    #region OperationResponse

    public const int GAME_START_RESPONSE = 0x00000200; //开始游戏_服务器响应

    public const int SUMMON_RETINUE_REQUEST_RESPONSE = 0x00000201; //召唤随从_服务器响应
    public const int EQUIP_WEAPON_REQUEST_RESPONSE = 0x00000202; //装备武器_服务器响应
    public const int EQUIP_SHIELD_REQUEST_RESPONSE = 0x00000203; //装备防具_服务器响应
    public const int USE_SPELLCARD_REQUEST_RESPONSE = 0x00000204; //使用法术_服务器响应

    public const int RETINUE_ATTACK_RETINUE_REQUEST_RESPONSE = 0x00000205; //随从攻击随从_服务器响应

    public const int END_ROUND_REQUEST_RESPONSE = 0x00000206; //结束回合_服务器响应

    #endregion

    #region SideEffects

    public const int SE_SET_PLAYER = 0x00000207; //英雄信息

    public const int SE_PLAYER_TURN = 0x00000208; //切换玩家
    public const int SE_PLAYER_COST_CHANGE = 0x00000209; //英雄费用信息
    public const int SE_PLAYER_LIFE_CHANGE = 0x00000210; //英雄生命信息
    public const int SE_PLAYER_MAGIC_CHANGE = 0x00000211; //英雄魔法信息

    public const int SE_RETINUE_ATTRIBUTES_CHANGE = 0x00000212; //随从数值变化

    public const int SE_RETINUE_DIE = 0x00000213; //随从死亡

    public const int SE_BATTLEGROUND_ADD_RETINUE = 0x00000214; //战场增加随从
    public const int SE_BATTLEGROUND_REMOVE_RETINUE = 0x00000215; //战场减少随从

    public const int SE_CARDDECT_LEFT_CHANGE = 0x00000216; //发送剩余牌数
    public const int SE_DRAW_CARD = 0x00000217; //抽一张牌
    public const int SE_DROP_CARD = 0x00000218; //手牌弃牌
    public const int SE_USE_CARD = 0x00000219; //手牌用牌

    public const int SE_EQUIP_WEAPON_SERVER_REQUEST = 0x00000220; //装备武器
    public const int SE_EQUIP_SHIELD_SERVER_REQUEST = 0x00000221; //装备防具
    public const int SE_USE_SPELLCARD_SERVER_REQUEST = 0x00000222; //使用法术

    public const int SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST = 0x00000223; //随从攻击随从
    public const int SE_DAMAGE_ONE_RETINUE_REQUEST = 0x00000224; //随从受到伤害
    public const int SE_RETINUE_EFFECT = 0x00000225; //随从特效

    #endregion

    #endregion

    #region ClientOperation

    public const int SUMMON_RETINUE_REQUEST = 0x00000301; //召唤随从
    public const int EQUIP_WEAPON_REQUEST = 0x00000302; //装备武器
    public const int EQUIP_SHIELD_REQUEST = 0x00000303; //装备防具
    public const int USE_SPELLCARD_REQUEST = 0x00000304; //使用法术

    public const int RETINUE_ATTACK_RETINUE_REQUEST = 0x00000305; //随从攻击随从

    public const int END_ROUND_REQUEST = 0x00000306; //结束回合

    #endregion

    #endregion
}