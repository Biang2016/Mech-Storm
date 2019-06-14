public class Battle
{
    public ILog DebugLog;
    internal GameManager GameManager;
    public BattleProxy ClientA;
    public BattleProxy ClientB;

    public Battle(BattleProxy clientA, BattleProxy clientB, ILog debugLog)
    {
        DebugLog = debugLog;
        ClientA = clientA;
        ClientB = clientB;
        GameManager = new GameManager(this, clientA, clientB);
        clientA.Battle = this;
        clientB.Battle = this;
    }

    public void BattleForceEnd()
    {
        GameManager.OnEndGameByServerError();
    }
}