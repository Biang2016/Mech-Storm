public enum NetProtocols
{
    #region  OutGame

    #region Server

    CLIENT_ID_REQUEST, //服务器发送的客户端ID号，协议号固定，不随版本变化
    HEART_BEAT_REQUEST, //服务器发送的心跳包
    CLIENT_BUILDINFOS_REQUEST, //玩家所有卡组信息
    GAME_STOP_BY_LEAVE_REQUEST, //由于玩家离开导致游戏结束
    GAME_STOP_BY_SERVER_ERROR_REQUEST, //由于服务器错误导致游戏结束
    GAME_STOP_BY_WIN_REQUEST, //由于比赛输赢导致游戏结束
    RANDOM_NUMBER_SEED_REQUEST, //分配客户端随机数种子
    REGISTER_RESULT_REQUEST, //注册结果，协议号固定，不随版本变化
    LOGIN_RESULT_REQUEST, //登录结果，协议号固定，不随版本变化
    LOGOUT_RESULT_REQUEST, //登出结果
    START_NEW_STORY_REQUEST_RESPONSE, //新故事回应
    REFRESH_STORY_REQUEST, //新故事回应
    CREATE_BUILD_REQUEST_RESPONSE, //新卡组回应
    BUILD_UPDATE_RESPONSE, //更新卡组
    DELETE_BUILD_REQUEST_RESPONSE, //删除卡组
    REFRESH_GAMEPLAYSETTINGS_REQUEST, //更新单人模式的GamePlaySettings

    VISIT_SHOP_REQUEST_RESPONSE, //光顾商店
    START_FIGHTING_ENEMY_REQUEST, //开打enemy
    BEAT_LEVEL_REQUEST, //过关

    #endregion

    # region Client

    CARD_DECK_REQUEST, //卡组信息
    MATCH_REQUEST, //申请匹配
    STANDALONE_START_LEVEL_REQUEST, //申请单机模式开始关卡
    CANCEL_MATCH_REQUEST, //玩家取消匹配
    CLIENT_VERSION_VALID_REQUEST, //客户端拿到服务器版本号后，如果版本匹配，给服务器返回此协议，如未返回此协议，在下条协议收到时将会踢出客户端连接
    REGISTER_REQUEST, //注册，协议号固定，不随版本变化
    LOGIN_REQUEST, //登录，协议号固定，不随版本变化
    LEAVE_GAME_REQUEST, //玩家离开游戏
    LOGOUT_REQUEST, //登出
    START_NEW_STORY_REQUEST, //开始新故事
    BUILD_REQUEST, //编辑卡组
    SINGLE_BUILD_REQUEST, //编辑单机卡组
    DELETE_BUILD_REQUEST, //删除卡组
    BONUS_GROUP_REQUEST, //奖励请求
    BUY_SHOP_ITEM_REQUEST, //购物请求
    LEAVE_SHOP_REQUEST, //离开商店

    #endregion

    #endregion

    #region InGame

    #region Server

    #region OperationResponse

    GAME_START_RESPONSE, //开始游戏_服务器响应

    SUMMON_MECH_REQUEST_RESPONSE, //召唤随从_服务器响应
    EQUIP_WEAPON_REQUEST_RESPONSE, //装备武器_服务器响应
    EQUIP_SHIELD_REQUEST_RESPONSE, //装备防具_服务器响应
    EQUIP_PACK_REQUEST_RESPONSE, //装备飞行背包_服务器响应
    EQUIP_MA_REQUEST_RESPONSE, //装备MA_服务器响应
    USE_SPELLCARD_REQUEST_RESPONSE, //使用法术_服务器响应

    MECH_ATTACK_MECH_REQUEST_RESPONSE, //随从攻击随从_服务器响应

    END_ROUND_REQUEST_RESPONSE, //结束回合_服务器响应
    WIN_DIRECTLY_REQUEST_RESPONSE, //直接获胜_服务器响应
    BUY_SHOP_ITEM_REQUEST_RESPONSE, // 购物请求返回

    #endregion

    #region SideEffects

    SE_SET_PLAYER, //英雄信息

    SE_PLAYER_TURN, //切换玩家
    SE_PLAYER_METAL_CHANGE, //英雄费用信息
    SE_PLAYER_LIFE_CHANGE, //英雄生命信息
    SE_PLAYER_ENERGY_CHANGE, //英雄能量信息

    SE_MECH_ATTRIBUTES_CHANGE, //随从数值变化

    SE_MECH_DIE, //随从死亡

    SE_BATTLEGROUND_ADD_MECH, //战场增加随从
    SE_BATTLEGROUND_REMOVE_MECH, //战场减少随从

    SE_PLAYER_BUFF_UPDATE_REQUEST, //玩家buff类SE的同步

    SE_PLAYER_COOLDOWNCARD_UPDATE_REQUEST, //玩家COOLDOWNCARD的同步
    SE_PLAYER_COOLDOWNCARD_REMOVE_REQUEST, //玩家COOLDOWNCARD的同步

    SE_CARD_ATTR_CHANGE, //卡牌信息变化
    SE_CARDDECT_LEFT_CHANGE, //发送剩余牌数
    SE_DRAW_CARD, //抽一张牌
    SE_DROP_CARD, //手牌弃牌
    SE_USE_CARD, //手牌用牌

    SE_MECH_CARDINFO_SYNC, //场上随从卡牌信息同步
    SE_HAND_CARDINFO_SYNC, //手牌卡牌信息同步
    SE_EQUIP_CARDINFO_SYNC, //场上装备卡牌信息同步

    SE_EQUIP_WEAPON_SERVER_REQUEST, //装备武器
    SE_EQUIP_SHIELD_SERVER_REQUEST, //装备防具
    SE_EQUIP_PACK_SERVER_REQUEST, //装备飞行背包
    SE_EQUIP_MA_SERVER_REQUEST, //装备MA
    SE_USE_SPELLCARD_SERVER_REQUEST, //使用法术
    SE_USE_SPELLCARD_TO_MECH_SERVER_REQUEST, //使用法术指向随从
    SE_USE_SPELLCARD_TO_EQUIP_SERVER_REQUEST, //使用法术指向装备
    SE_USE_SPELLCARD_TO_SHIP_SERVER_REQUEST, //使用法术指向战舰

    SE_MECH_ATTACK_MECH_SERVER_REQUEST, //随从攻击随从
    SE_MECH_ATTACK_SHIP_SERVER_REQUEST, //随从攻击战舰
    SE_MECH_ONATTACK, //随从发起进攻动作
    SE_MECH_ONATTACKSHIP, //随从对战舰发起进攻动作
    SE_MECH_SHIELD_DEFENSE, //随从护盾抵消攻击
    SE_MECH_CANATTACK, //随从能否进攻
    SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT, //特效触发通知

    SE_MECH_IMMUNE, //随从免疫信息通知
    SE_MECH_INACTIVITY, //随从无法行动信息通知

    #endregion

    #endregion

    #region ClientOperation

    SUMMON_MECH_REQUEST, //召唤随从
    EQUIP_WEAPON_REQUEST, //装备武器
    EQUIP_SHIELD_REQUEST, //装备防具
    EQUIP_PACK_REQUEST, //装备飞行背包
    EQUIP_MA_REQUEST, //装备MA
    USE_SPELLCARD_REQUEST, //使用法术
    USE_SPELLCARD_TO_MECH_REQUEST, //使用法术指向随从
    USE_SPELLCARD_TO_EQUIP_REQUEST, //使用法术指向装备
    USE_SPELLCARD_TO_SHIP_REQUEST, //使用法术指向战舰

    MECH_ATTACK_MECH_REQUEST, //随从攻击随从
    MECH_ATTACK_SHIP_REQUEST, //随从攻击战舰

    END_ROUND_REQUEST, //结束回合
    END_BATTLE_REQUEST, //结束战斗
    WIN_DIRECTLY_REQUEST, //直接赢得战斗（GM协议）

    #endregion

    #endregion
}