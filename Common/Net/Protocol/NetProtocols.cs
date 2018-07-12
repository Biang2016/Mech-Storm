using System.Collections;

public class NetProtocols
{
    public const int TEST_CONNECT = 0x00000001; //连接测试
    public const int SEND_CLIENT_ID = 0x00000002; //发送客户端Id
    public const int RESET_GAME = 0x00000005; //请求重置游戏

    public const int INFO_NUMBER = 0x00000003; //服务端信息
    public const int WARNING_NUMBER = 0x00000004; //服务端警示

    public const int Match = 0x00000099; //申请匹配
    public const int PLAYER = 0x00000101; //英雄信息
    public const int PLAYER_COST_CHANGE = 0x00000102; //英雄费用信息
    public const int DRAW_CARD = 0x00000103; //抽一张牌
    public const int SUMMON_RETINUE = 0x00000105; //召唤随从
    public const int SUMMON_RETINUE_RESPONSE = 0x00000109; //召唤随从_服务器响应
    public const int PLAYER_TURN = 0x00000106; //切换玩家
    public const int CLIENT_END_ROUND = 0x00000107; //切换玩家
    public const int CARD_DECK_INFO = 0x00000108; //卡组信息
}