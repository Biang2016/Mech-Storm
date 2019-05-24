public class Battle
{
    private GameManager GameManager;
    public BattleClientProxy ClientA;
    public BattleClientProxy ClientB;

    public Battle(BattleClientProxy clientA, BattleClientProxy clientB, BattleClientProxy.DoSendToClientDelegate doSendToClientDelegate)
    {
        ClientA = clientA;
        ClientB = clientB;
        GameManager = new GameManager(clientA, clientB);
        clientA.DoSendToClient = doSendToClientDelegate;
        clientB.DoSendToClient = doSendToClientDelegate;
    }
}