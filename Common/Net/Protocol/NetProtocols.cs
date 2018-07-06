using System.Collections;

public class NetProtocols    
{
    public static int TEST_CONNECT = 0x00000001;//连接测试
    public static int ENTRY_GAME = 0x00000100;//登录游戏

    public static int PLAYER = 0x00000101;//英雄信息
    public static int PLAYER_COST_CHANGE = 0x00000102;//英雄费用信息
    public static int GET_A_CARD = 0x00000103;//获取一张牌
    public static int SUMMON_RETINUE = 0x00000104;//召唤随从
    public static int PLAYER_TURN = 0x00000105;//切换玩家
    public static int CARD_DECK_INFO = 0x00000106;//卡组信息
    public static int DRAW_CARD = 0x00000107;//抽一张牌
}
