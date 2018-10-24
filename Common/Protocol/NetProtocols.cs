public enum NetProtocols
{
    #region  OutGame

    #region Server

    CLIENT_ID_REQUEST, //服务器发送的客户端ID号
    HEART_BEAT_REQUEST, //服务器发送的心跳包
    CLIENT_BUILDINFOS_REQUEST, //玩家所有卡组信息
    GAME_STOP_BY_LEAVE_REQUEST, //由于玩家离开导致游戏结束
    GAME_STOP_BY_WIN_REQUEST, //由于比赛输赢导致游戏结束
    RANDOM_NUMBER_SEED_REQUEST, //分配客户端随机数种子
    REGISTER_RESULT_REQUEST, //注册结果
    LOGIN_RESULT_REQUEST, //登录结果
    LOGOUT_RESULT_REQUEST, //登出结果
    CREATE_BUILD_REQUEST_RESPONSE, //新卡组回应
    BUILD_UPDATE_RESPONSE, //更新卡组
    DELETE_BUILD_REQUEST_RESPONSE, //删除卡组

    #endregion

    # region Client

    CARD_DECK_REQUEST, //卡组信息
    MATCH_REQUEST, //申请匹配
    CANCEL_MATCH_REQUEST, //玩家取消匹配
    LEAVE_GAME_REQUEST, //玩家离开游戏
    REGISTER_REQUEST, //注册
    LOGIN_REQUEST, //登录
    LOGOUT_REQUEST, //登出
    BUILD_REQUEST, //编辑卡组
    DELETE_BUILD_REQUEST, //删除卡组

    #endregion

    #endregion

    #region InGame

    #region Server

    #region OperationResponse

    GAME_START_RESPONSE, //开始游戏_服务器响应

    SUMMON_RETINUE_REQUEST_RESPONSE, //召唤随从_服务器响应
    EQUIP_WEAPON_REQUEST_RESPONSE, //装备武器_服务器响应
    EQUIP_SHIELD_REQUEST_RESPONSE, //装备防具_服务器响应
    EQUIP_PACK_REQUEST_RESPONSE, //装备飞行背包_服务器响应
    EQUIP_MA_REQUEST_RESPONSE, //装备MA_服务器响应
    USE_SPELLCARD_REQUEST_RESPONSE, //使用法术_服务器响应

    RETINUE_ATTACK_RETINUE_REQUEST_RESPONSE, //随从攻击随从_服务器响应

    END_ROUND_REQUEST_RESPONSE, //结束回合_服务器响应

    #endregion

    #region SideEffects

    SE_SET_PLAYER, //英雄信息

    SE_PLAYER_TURN, //切换玩家
    SE_PLAYER_METAL_CHANGE, //英雄费用信息
    SE_PLAYER_LIFE_CHANGE, //英雄生命信息
    SE_PLAYER_ENERGY_CHANGE, //英雄能量信息

    SE_RETINUE_ATTRIBUTES_CHANGE, //随从数值变化

    SE_RETINUE_DIE, //随从死亡

    SE_BATTLEGROUND_ADD_RETINUE, //战场增加随从
    SE_BATTLEGROUND_REMOVE_RETINUE, //战场减少随从

    SE_PLAYER_BUFF_UPDATE_REQUEST, //玩家buff类SE的同步
    SE_PLAYER_BUFF_REMOVE_REQUEST, //玩家buff类SE的同步

    SE_CARD_ATTR_CHANGE, //卡牌信息变化
    SE_CARDDECT_LEFT_CHANGE, //发送剩余牌数
    SE_DRAW_CARD, //抽一张牌
    SE_DROP_CARD, //手牌弃牌
    SE_USE_CARD, //手牌用牌

    SE_RETINUE_CARDINFO_SYNC, //场上随从卡牌信息同步
    SE_HAND_CARDINFO_SYNC, //手牌卡牌信息同步
    SE_EQUIP_CARDINFO_SYNC, //场上装备卡牌信息同步

    SE_EQUIP_WEAPON_SERVER_REQUEST, //装备武器
    SE_EQUIP_SHIELD_SERVER_REQUEST, //装备防具
    SE_EQUIP_PACK_SERVER_REQUEST, //装备飞行背包
    SE_EQUIP_MA_SERVER_REQUEST, //装备MA
    SE_USE_SPELLCARD_SERVER_REQUEST, //使用法术
    SE_USE_SPELLCARD_TO_RETINUE_SERVER_REQUEST, //使用法术指向随从
    SE_USE_SPELLCARD_TO_EQUIP_SERVER_REQUEST, //使用法术指向装备
    SE_USE_SPELLCARD_TO_SHIP_SERVER_REQUEST, //使用法术指向战舰

    SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST, //随从攻击随从
    SE_RETINUE_ATTACK_SHIP_SERVER_REQUEST, //随从攻击战舰
    SE_RETINUE_DODGE, //随从成功闪避
    SE_RETINUE_ONATTACK, //随从发起进攻动作
    SE_RETINUE_CANATTACK, //随从能否进攻
    SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT, //特效触发通知

    #endregion

    #endregion

    #region ClientOperation

    SUMMON_RETINUE_REQUEST, //召唤随从
    EQUIP_WEAPON_REQUEST, //装备武器
    EQUIP_SHIELD_REQUEST, //装备防具
    EQUIP_PACK_REQUEST, //装备飞行背包
    EQUIP_MA_REQUEST, //装备MA
    USE_SPELLCARD_REQUEST, //使用法术
    USE_SPELLCARD_TO_RETINUE_REQUEST, //使用法术指向随从
    USE_SPELLCARD_TO_EQUIP_REQUEST, //使用法术指向装备
    USE_SPELLCARD_TO_SHIP_REQUEST, //使用法术指向战舰

    RETINUE_ATTACK_RETINUE_REQUEST, //随从攻击随从
    RETINUE_ATTACK_SHIP_REQUEST, //随从攻击战舰

    END_ROUND_REQUEST, //结束回合

    #endregion

    #endregion
}