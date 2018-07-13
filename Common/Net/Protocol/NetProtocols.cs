using System.Collections;

public class NetProtocols
{
    public const int HEART_BEAT = 0x00000001; //服务器发送的心跳包
    public const int SEND_CLIENT_ID = 0x00000002; //分配客户端Id
    public const int GAME_STOP_BY_LEAVE = 0x00000003; //由于玩家离开导致游戏结束

    public const int PLAYER = 0x00000101; //英雄信息
    public const int PLAYER_TURN = 0x00000102; //切换玩家
    public const int DRAW_CARD = 0x00000103; //抽一张牌
    public const int PLAYER_COST_CHANGE = 0x00000104; //英雄费用信息
    public const int SUMMON_RETINUE_RESPONSE = 0x00000105; //召唤随从_服务器响应

    public const int Match = 0x00000201; //申请匹配
    public const int CANCEL_MATCH = 0x00000202; //玩家取消匹配
    public const int LEAVE_GAME = 0x00000203; //玩家离开游戏
    public const int END_ROUND = 0x00000204; //结束回合
    public const int SUMMON_RETINUE = 0x00000205; //召唤随从
    public const int CARD_DECK_INFO = 0x00000206; //卡组信息

}